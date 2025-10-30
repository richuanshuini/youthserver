using YouthApartmentServer.Model.ServersModel;
using YouthApartmentServer.ModelDto;

namespace YouthApartmentServer.Profiles;

public static class AnnounceMentProfile
{
    public static void Register(Mapster.TypeAdapterConfig config)
    {
        // 查询：实体 -> DTO
        config.NewConfig<AnnounceMent, AnnouncementDto>()
            .Map(dest => dest.Type, src => (int)src.Type)
            .Map(dest => dest.Status, src => (int)src.Status);

        // 插入：DTO -> 实体（int 到 enum 自动转换）
        config.NewConfig<InsertAnnouncementDto, AnnounceMent>();
    }
}