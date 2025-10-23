# 青年公寓管理系统 项目架构与开发指南

适用范围：中小型个人开发项目；不引入复杂身份认证；不做高并发优化。本指南帮助你在当前分层架构下，按“只增不改”的方式持续扩展功能。

## 设计原则
- 控制器瘦身：只做路由、参数校验、授权（如有）、调用服务并返回结果。
- 业务内聚：服务层承载业务规则与流程，控制事务边界。
- 数据访问单一：仓储层使用 FreeSql 负责查询与 CRUD。
- DTO/实体分离：读写分离；读场景投影到 DTO，写场景使用映射。
- 开放封闭：新增功能通过新增文件实现，不修改既有代码。

## 目录与职责
- `Controller/`：API 路由入口；不直接写 SQL，不承载复杂业务。
- `Services/`：业务逻辑与流程编排；依赖仓储与 AutoMapper；控制事务边界。
- `Repositories/`：FreeSql 数据访问封装；公开实体级查询与 CRUD。
- `Model/`：数据库实体模型；可包含审计字段与软删除。
- `ModelDto/`：请求/响应对象（DTO），避免直接暴露实体。
- `Profiles/`：AutoMapper 配置（写场景映射）；读场景优先 FreeSql 投影。
- `Common/`：统一响应、分页模型、异常与工具类。

## 工作流（读/写）
- 读（GET）：Controller → Service → Repository `Select<TEntity>` → `ToList<Dto>()` → `ApiResponse`。
- 写（POST/PUT）：Controller → Service（校验 & `Mapper`→实体）→ UnitOfWork → Repository `Insert/Update` → `ApiResponse`。

## 新增功能流程（模板）
按下列步骤新增一个模块，即可实现“只增不改”。以 `Room` 为例：

### 1) 实体（Model/Room.cs）
```csharp
namespace YouthApartmentServer.Model;

public class Room : BaseEntity
{
    public string Number { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public bool IsOccupied { get; set; }
}
```

### 2) DTO（ModelDto/RoomDto.cs, RoomCreateDto.cs）
```csharp
namespace YouthApartmentServer.ModelDto;

public class RoomDto
{
    public long Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public bool IsOccupied { get; set; }
}

public class RoomCreateDto
{
    public string Number { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
}
```

### 3) 映射配置（Profiles/RoomProfile.cs）
```csharp
using AutoMapper;
using YouthApartmentServer.Model;
using YouthApartmentServer.ModelDto;

public class RoomProfile : Profile
{
    public RoomProfile()
    {
        CreateMap<Room, RoomDto>();
        CreateMap<RoomCreateDto, Room>();
    }
}
```

### 4) 仓储（Repositories/IRoomRepository.cs, RoomRepository.cs）
```csharp
using FreeSql;
using YouthApartmentServer.Model;

public interface IRoomRepository
{
    ISelect<Room> Queryable();
    Task<long> InsertAsync(Room entity);
    Task<int> UpdateAsync(Room entity);
    Task<Room?> GetByIdAsync(long id);
}

public class RoomRepository : IRoomRepository
{
    private readonly IFreeSql _fsql;
    public RoomRepository(IFreeSql fsql) => _fsql = fsql;

    public ISelect<Room> Queryable() => _fsql.Select<Room>();
    public Task<long> InsertAsync(Room e) => _fsql.Insert(e).ExecuteIdentityAsync();
    public Task<int> UpdateAsync(Room e) => _fsql.Update<Room>().SetSource(e).ExecuteAffrowsAsync();
    public Task<Room?> GetByIdAsync(long id) => _fsql.Select<Room>().Where(x => x.Id == id).FirstAsync();
}
```

### 5) 服务（Services/IRoomService.cs, RoomService.cs）
```csharp
using AutoMapper;
using YouthApartmentServer.Model;
using YouthApartmentServer.ModelDto;

public interface IRoomService
{
    Task<List<RoomDto>> GetListAsync(string? building);
    Task<long> CreateAsync(RoomCreateDto dto);
}

public class RoomService : IRoomService
{
    private readonly IRoomRepository _repo;
    private readonly IMapper _mapper;
    private readonly IFreeSql _fsql;

    public RoomService(IRoomRepository repo, IMapper mapper, IFreeSql fsql)
    { _repo = repo; _mapper = mapper; _fsql = fsql; }

    public Task<List<RoomDto>> GetListAsync(string? building)
    {
        var q = _repo.Queryable();
        if (!string.IsNullOrWhiteSpace(building)) q = q.Where(x => x.Building == building);
        return q.ToListAsync<RoomDto>(); // 读场景：直接投影到 DTO
    }

    public async Task<long> CreateAsync(RoomCreateDto dto)
    {
        using var uow = _fsql.CreateUnitOfWork(); // 写场景：事务边界在服务层
        var repo = uow.GetRepository<Room>(); // 或直接使用 _repo + uow 控制
        var entity = _mapper.Map<Room>(dto);
        var id = await _repo.InsertAsync(entity);
        uow.Commit();
        return id;
    }
}
```

### 6) 控制器（Controller/RoomController.cs）
```csharp
using Microsoft.AspNetCore.Mvc;
using YouthApartmentServer.Common; // ApiResponse
using YouthApartmentServer.ModelDto;

[ApiController]
[Route("api/[controller]")]
public class RoomController : ControllerBase
{
    private readonly IRoomService _service;
    public RoomController(IRoomService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<RoomDto>>>> List([FromQuery] string? building)
        => ApiResponse<List<RoomDto>>.Ok(await _service.GetListAsync(building));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> Create([FromBody] RoomCreateDto dto)
        => ApiResponse<long>.Ok(await _service.CreateAsync(dto));
}
```

### 7) Swagger 注解（可选）
在控制器方法上使用：
```csharp
using Swashbuckle.AspNetCore.Annotations;

[SwaggerOperation(Summary = "房间列表", Description = "按楼栋筛选房间")]
[SwaggerResponse(200, "成功", typeof(List<RoomDto>))]
```

## 通用模型（建议）
- 统一响应（Common/ApiResponse.cs）：
```csharp
namespace YouthApartmentServer.Common;
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? ErrorCode { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public static ApiResponse<T> Ok(T data) => new() { Success = true, Data = data };
    public static ApiResponse<T> Fail(string code, string msg) => new() { Success = false, ErrorCode = code, Message = msg };
}
```
- 基类实体（Model/BaseEntity.cs）：
```csharp
namespace YouthApartmentServer.Model;
public abstract class BaseEntity
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
}
```
- 分页模型（Common/Paging.cs）：
```csharp
public class PageRequest { public int PageIndex { get; set; } = 1; public int PageSize { get; set; } = 20; }
public class PageResult<T> { public long Total { get; set; } public List<T> Items { get; set; } = new(); }
```

## Program.cs 依赖注册（示例）
仅作参考，不要求立即修改：
```csharp
// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// FreeSql（读取配置）
var conn = builder.Configuration.GetConnectionString("Default");
builder.Services.AddSingleton<IFreeSql>(_ => new FreeSql.FreeSqlBuilder()
    .UseConnectionString(FreeSql.DataType.MySql, conn)
    .UseAutoSyncStructure(false)
    .Build());
// 约定式注册（示例：扫描并按接口注入 Service/Repository）
// builder.Services.AddAppConventions();
```

## 规范与建议
- 控制器不直接依赖 `IFreeSql`；所有数据访问经由仓储。
- 读场景优先使用 FreeSql 的投影到 DTO；写场景用 AutoMapper。
- 事务边界在服务层；使用 `UnitOfWork` 保证一致性。
- 统一响应与异常处理，保持前端交互一致。
- 新增模块按模板新增文件，无需修改既有代码与启动配置。

## Repositories 与 Services 的边界与示例

### 边界原则
- Repository（仓储）：
  - 专注数据访问（CRUD、查询构造、分页、排序），不承载业务规则。
  - 不开事务、不跨实体写入、不做复杂校验；返回实体或 `ISelect<TEntity>` 供上层继续组合。
  - 读场景可直接 `ToList<Dto>()` 投影（仅字段选择），但不决定“是否允许”的业务语义。
- Service（服务）：
  - 承载业务规则与流程编排（跨实体、跨仓储），决定“可以/不可以”。
  - 控制事务边界（UnitOfWork），一致性由服务负责；统一异常与错误码。
  - 组合仓储查询，组装输出 DTO 或统一响应；写场景用 AutoMapper 做 DTO→实体。

### 示例 A：创建租约（业务规则 + 事务）

Repository（只负责数据查询/写入，不决定业务）：
```csharp
public interface IContractRepository {
    Task<bool> ExistsOverlapAsync(long roomId, DateTime start, DateTime end);
    Task<long> InsertAsync(Contract contract);
}

public class ContractRepository : IContractRepository {
    private readonly IFreeSql _fsql;
    public Task<bool> ExistsOverlapAsync(long roomId, DateTime start, DateTime end)
        => _fsql.Select<Contract>()
                .Where(c => c.RoomId == roomId && !c.IsDeleted)
                .Where(c => c.StartDate <= end && c.EndDate >= start)
                .AnyAsync();
    public Task<long> InsertAsync(Contract c) => _fsql.Insert(c).ExecuteIdentityAsync();
}
```

Service（决定业务、开事务、协调多仓储）：
```csharp
public class ContractService : IContractService {
    private readonly IContractRepository _contracts;
    private readonly ILedgerRepository _ledger;
    private readonly IFreeSql _fsql;

    public async Task<long> CreateAsync(ContractCreateDto dto) {
        if (dto.StartDate >= dto.EndDate)
            throw new AppException("CONTRACT_RANGE_INVALID", "起止日期不合法");
        if (await _contracts.ExistsOverlapAsync(dto.RoomId, dto.StartDate, dto.EndDate))
            throw new AppException("CONTRACT_OVERLAP", "同房间存在重叠租约");

        using var uow = _fsql.CreateUnitOfWork();
        var id = await _contracts.InsertAsync(new Contract { /*...*/ });
        await _ledger.InsertAsync(new Ledger { ContractId = id, Amount = dto.Deposit, Type = LedgerType.Deposit });
        uow.Commit();
        return id;
    }
}
```
要点：重叠判断可由仓储提供查询能力，但“能否创建”由服务层裁决并在事务中完成整体写入。

### 示例 B：房间列表查询（组合筛选 + 投影）

Repository（提供基础查询，不承载前端筛选细节）：
```csharp
public interface IRoomRepository {
    ISelect<Room> Query();
}

public class RoomRepository : IRoomRepository {
    private readonly IFreeSql _fsql;
    public ISelect<Room> Query() => _fsql.Select<Room>().Where(r => !r.IsDeleted);
}
```

Service（组合筛选，决定排序，投影到 DTO）：
```csharp
public Task<List<RoomDto>> GetListAsync(RoomListRequest req) {
    var q = _rooms.Query();
    if (!string.IsNullOrWhiteSpace(req.Building)) q = q.Where(r => r.Building == req.Building);
    if (req.OnlyVacant) q = q.Where(r => !r.IsOccupied);
    return q.OrderBy(r => r.Number).ToListAsync<RoomDto>();
}
```
要点：仓储负责返回干净的可组合查询；服务根据需求拼装条件与输出。

### 示例 C：不要放到仓储的逻辑（反例）
- 在仓储中执行“创建租约 + 记账 + 发送通知”这样的跨实体/跨边界操作（应在服务层）。
- 在仓储中开启/提交事务（应由服务层控制，确保跨仓储一致性）。
- 在仓储中抛出业务错误码（仓储只抛技术异常；业务错误由服务统一裁决）。

> 本指南不涵盖复杂的身份认证与高并发优化，聚焦于中小型项目的清晰分层与可持续扩展。