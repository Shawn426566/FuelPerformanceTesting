using FuelWebUi.ApiClient.Models;
using FuelWebUi.Pages.Crud.Templates;
using FuelWebUi.Pages.Crud.FormModels;

namespace FuelWebUi.Pages.Crud.Implmentations
{
    public partial class TeamsCrudPage : TemplateCrudPage<TeamListDto, TeamFormModel, Team>
    {
        protected override string GetEntityName() => "Team";

        protected override dynamic EntityApi => _apiClient.Api.Teams;

        protected override int? GetSelectItemId()
        {
            return _selectedItem?.TeamId;
        }

        protected override TeamFormModel MapSelectedItemToFormModel()
        {
            return new TeamFormModel
            {
                Name = _selectedItem?.Name ?? string.Empty,
                AssociationID = _selectedItem?.AssociationId
            };
        }

        protected override Team MapToEntityForAdd()
        {
            return new Team
            {
                Name = _formModel?.Name ?? string.Empty,
                AssociationID = _formModel?.AssociationID
            };
        }

        protected override Team MapToEntityForUpdate()
        {
            return new Team
            {
                TeamID = _selectedId,
                Name = _formModel?.Name ?? string.Empty,
                AssociationID = _formModel?.AssociationID
            };
        }
    }
}
