using FiorelloOneToMany.Data;
using FiorelloOneToMany.Models;
using FiorelloOneToMany.Services.Interfaces;
using FiorelloOneToMany.VıewModels;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Security.Claims;

namespace FiorelloOneToMany.Services
{
    public class LayoutService : ILayoutService
    {
        private readonly AppDbContext _context;
   
        private readonly IBasketService _basketService;
        private readonly HttpContextAccessor _httpContextAccessor;

        private readonly UserManager<AppUser> _userManager;
        public LayoutService(AppDbContext context ,IBasketService basketService,HttpContextAccessor httpContextAccessor,UserManager<AppUser> userManager)
        {
            _context = context;
        
            _basketService = basketService;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }


        public async Task<LayoutVM> GetAllDatas()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(userId);

             

            int count = _basketService.GetCount();
         var datas= _context.Settings.AsEnumerable().ToDictionary(m=>m.Key,m=>m.Value);
            return new LayoutVM { BasketCount=count,SettingDatas=datas,UserFullName=user?.FullName};
         
        }


    }
}
