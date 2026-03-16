using FuelWebUi.ApiClient.Models;
using FuelWebUi.Pages.Crud.Templates;
using FuelWebUi.Pages.Crud.FormModels;

namespace FuelWebUi.Pages.Crud.Implmentations
{
    public partial class StaffMembersCrudPage : TemplateCrudPage<StaffMemberListDto, StaffMemberFormModel, StaffMember>
    {
        protected override string GetEntityName() => "Staff Member";

        protected override dynamic EntityApi => _apiClient.Api.StaffMembers;

        protected override int? GetSelectItemId()
        {
            return _selectedItem?.StaffMemberId;
        }

        protected override StaffMemberFormModel MapSelectedItemToFormModel()
        {
            return new StaffMemberFormModel
            {
                FirstName = _selectedItem?.FirstName ?? string.Empty,
                LastName = _selectedItem?.LastName ?? string.Empty,
                Email = _selectedItem?.Email,
                Role = Enum.TryParse<StaffRole>(_selectedItem?.Role, out var role) ? role : (StaffRole?)null,
                TeamID = _selectedItem?.TeamId
            };
        }

        protected override StaffMember MapToEntityForAdd()
        {
            return new StaffMember
            {
                FirstName = _formModel?.FirstName ?? string.Empty,
                LastName = _formModel?.LastName ?? string.Empty,
                Email = _formModel?.Email,
                Role = _formModel?.Role,
                TeamID = _formModel?.TeamID
            };
        }

        protected override StaffMember MapToEntityForUpdate()
        {
            return new StaffMember
            {
                StaffMemberID = _selectedId,
                FirstName = _formModel?.FirstName ?? string.Empty,
                LastName = _formModel?.LastName ?? string.Empty,
                Email = _formModel?.Email,
                Role = _formModel?.Role,
                TeamID = _formModel?.TeamID
            };
        }
    }
}
