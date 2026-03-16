using FuelWebUi.ApiClient.Models;
using FuelWebUi.Pages.Crud.Templates;
using FuelWebUi.Pages.Crud.FormModels;

namespace FuelWebUi.Pages.Crud.Implmentations
{
	public partial class EvaluationsCrudPage : TemplateCrudPage<EvaluationListDto, EvaluationFormModel, Evaluation>
	{
		protected override string GetEntityName() => "Evaluation";

		protected override dynamic EntityApi => _apiClient.Api.Evaluations;

		protected override int? GetSelectItemId()
		{
			return _selectedItem?.EvaluationId;
		}

		protected override EvaluationFormModel MapSelectedItemToFormModel()
		{
			return new EvaluationFormModel
			{
				Date = ToDateTime(_selectedItem?.Date),
				PlayerId = _selectedItem?.PlayerId,
				StaffMemberId = _selectedItem?.StaffMemberId,
				Summary = _selectedItem?.Summary ?? string.Empty
			};
		}

		protected override Evaluation MapToEntityForAdd()
		{
			return new Evaluation
			{
				Date = ToDateTime(_formModel?.Date),
				PlayerId = _formModel?.PlayerId,
				StaffMemberId = _formModel?.StaffMemberId,
				Summary = _formModel?.Summary ?? string.Empty
			};
		}

		protected override Evaluation MapToEntityForUpdate()
		{
			return new Evaluation
			{
				EvaluationId = _selectedId,
				Date = ToDateTime(_formModel?.Date),
				PlayerId = _formModel?.PlayerId,
				StaffMemberId = _formModel?.StaffMemberId,
				Summary = _formModel?.Summary ?? string.Empty
			};
		}
	}
}
