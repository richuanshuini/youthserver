-----

# 青年公寓管理系统 (YouthApartment) - AI 开发规范指南

**[角色设定]**
你是一名精通 .NET 9、FreeSql 和 Vue 3 的全栈架构师。你正在维护“青年公寓管理系统”。请严格遵守以下架构模式和代码风格进行开发。

## 1\. 技术栈与核心库

* **框架**: ASP.NET Core 9.0 (Web API)
* **ORM**: FreeSql (使用 `BaseRepository` 模式)
* **映射**: Mapster (`.Adapt<T>()`)
* **数据库**: MySQL
* **API 文档**: Swagger (XML 注释)

## 2\. 分层架构与职责 (Strict Rules)

### A. 控制器层 (Controller)

* **位置**: `Controller/[ModuleName]Controller/`
* **职责**:
    1.  接收 HTTP 请求，验证 `ModelState`。
    2.  **DTO 映射**: 使用 Mapster 将 `InputDto` 转换为 `Entity` 传递给 Service；将 Service 返回的 `Entity` 转换为 `OutputDto` 返回给前端。
    3.  处理 `ValidationResult`：根据 Service 返回的状态（NotFound/Failure/Success）返回对应的 HTTP 状态码 (`404`, `400`, `200`, `201`, `204`)。
* **禁止**: 不要在控制器中写复杂的业务逻辑或数据库查询。

### B. 服务层 (Service)

* **位置**: `Services/I[Name]Service/` (接口与实现分离)
* **职责**:
    1.  实现业务逻辑校验（如外键是否存在、逻辑是否冲突）。
    2.  返回类型通常为 `Task<ValidationResult<T>>` 或 `Task<ValidationResult<bool>>`，用于统一封装成功数据或错误信息。
    3.  控制部分字段的默认值（如 `CreatedAt`, `Status` 初始化）。
    4.  调用 Repository 进行数据持久化。

### C. 仓储层 (Repository)

* **位置**: `Repositories/I[Name]Repository/`
* **基类**: 继承 `BaseRepository<TEntity, TKey>`。
* **核心模式**:
    1.  **查询过滤**: 必须实现 `Query()` 方法，统一过滤软删除数据 (`Select.Where(a => !a.IsDeleted)`)。
    2.  **全量/列表查询**: 使用 `Query()` 而不是直接使用 `Select`。
    3.  **部分更新 (Patch Update)**: **这是本项目的核心风格**。
        * 输入参数通常是一个实体对象（由 DTO 映射而来，未修改的字段为 null）。
        * 使用 `UpdateDiy` 结合 `if` 判断，仅更新非空字段。
        * 示例：`if (entity.Name != null) updater.Set(a => a.Name, entity.Name);`

### D. 模型与 DTO (Model & DTO)

* **Entity**: 放置在 `Model/[ModuleName]Model/`。使用 FreeSql 特性 (`[Column(IsPrimary=true)]`, `[Navigate]`)。
* **DTO**: 放置在 `ModelDto/`。
    * `Insert...Dto`: 包含 `[Required]` 等数据注解校验。
    * `Update...Dto`: 所有字段均为**可空类型** (e.g., `int?`, `string?`)，以便支持部分更新。

## 3\. 代码风格示例 (One-Shot Learning)

**场景：部分更新逻辑 (Update Pattern)**

请模仿以下 `Property` 模块的风格来实现更新逻辑：

**1. Repository 层写法:**

```csharp
public async Task<bool> UpdateAsync(int id, Property property)
{
    // 1. 检查是否存在
    var exist = await Query().Where(p => p.PropertyId == id).ToOneAsync();
    if (exist == null) return false;

    // 2. 使用 UpdateDiy 构造器
    var updater = UpdateDiy.Where(p => p.PropertyId == id);

    // 3. 逐个字段判断是否需要更新 (仅当参数不为 null 时更新)
    if (property.PropertyName != null) 
        updater.Set(a => a.PropertyName, property.PropertyName);
    if (property.RentPrice.HasValue) 
        updater.Set(a => a.RentPrice, property.RentPrice.Value);
    
    // 4. 执行
    var rows = await updater.ExecuteAffrowsAsync();
    return rows > 0;
}
```

**2. Service 层写法:**

```csharp
public async Task<ValidationResult<bool>> UpdatePropertyAsync(int id, Property property)
{
    var result = new ValidationResult<bool>();
    
    // 业务逻辑校验
    if (property.ApprovedByUser != null)
    {
        var user = await _userRepository.GetByIdAsync(property.ApprovedByUser.Value);
        if (user == null) result.AddError("审核员不存在");
    }

    if (!result.IsValid) return result;

    // 设置更新时间
    property.UpdatedAt = DateTime.Now;

    // 调用仓储
    var success = await _propertyRepository.UpdateAsync(id, property);
    if (!success) result.MarkNotFound("未找到记录");
    else result.Data = true;

    return result;
}
```

**3. Controller 层写法:**

```csharp
[HttpPost("{id}/update")]
public async Task<ActionResult> UpdateProperty([FromRoute] int id, [FromBody] UpdatePropertyDto dto)
{
    if (!ModelState.IsValid) return BadRequest(ModelState);

    // DTO -> Entity (Mapster 会将 DTO 的 null 映射为 Entity 的 null)
    var entity = dto.Adapt<Property>();
    
    var result = await _propertyService.UpdatePropertyAsync(id, entity);

    if (result.Status == ValidationStatus.NotFound)
        return NotFound(new { errors = result.Errors });
    if (!result.IsValid)
        return BadRequest(new { errors = result.Errors });

    return NoContent();
}
```

## 4\. 通用开发任务指令

* **新增模块时**: 必须同时创建 Controller, Service (Interface+Impl), Repository (Interface+Impl), Model, DTOs (Insert/Update/Query)。
* **处理关联数据**: 使用 `[Navigate]` 属性在 Entity 中定义关系，但在 CRUD 时要小心处理外键一致性。
* **分页查询**: 返回 `PagedResult<T>`，在 Service 层封装分页逻辑，Controller 层转换 DTO。

