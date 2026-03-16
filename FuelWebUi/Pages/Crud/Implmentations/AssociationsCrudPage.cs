using FuelWebUi.ApiClient.Models;
using FuelWebUi.Pages.Crud.Templates;
using FuelWebUi.Pages.Crud.FormModels;

namespace FuelWebUi.Pages.Crud.Implmentations
{
    public partial class AssociationsCrudPage : TemplateCrudPage<AssociationListDto, AssociationFormModel, Association>
    {
        protected override string GetEntityName() => "Association";

        protected override dynamic EntityApi => _apiClient.Api.Associations;

        protected override int? GetSelectItemId()
        {
            return _selectedItem?.AssociationId;
        }

        protected override AssociationFormModel MapSelectedItemToFormModel()
        {
            return new AssociationFormModel
            {
                Name = _selectedItem?.Name ?? string.Empty
            };
        }

        protected override Association MapToEntityForAdd()
        {
            return new Association
            {
                Name = _formModel?.Name ?? string.Empty
            };
        }

        protected override Association MapToEntityForUpdate()
        {
            return new Association
            {   
                AssociationID = _selectedId,
                Name = _formModel?.Name ?? string.Empty
            };
        }
    }
}
