using FiorelloOneToMany.VıewModels;

namespace FiorelloOneToMany.Services.Interfaces
{
    public interface ILayoutService
    {
     Task<LayoutVM> GetAllDatas();
    }
}
