using Microsoft.AspNetCore.Mvc;
using YouthApartmentServer.Services.IPermissionService;

namespace YouthApartmentServer.Controller.BaseController;
[ApiController]
[Route("api/[controller]")]
public class PermissionController : ControllerBase
{
    private readonly PermissionService _permissionService;

    public PermissionController(PermissionService permissionService)
    {
        _permissionService = permissionService;
    }
    
    
    
    
    
}