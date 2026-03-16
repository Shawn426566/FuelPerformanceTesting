using FuelWebUi.ApiClient.Models;
using FuelWebUi.Pages.Crud.Templates;
using FuelWebUi.Pages.Crud.FormModels;

namespace FuelWebUi.Pages.Crud.Implmentations
{
	public partial class PlayersCrudPage : TemplateCrudPage<PlayerListDto, PlayerFormModel, Player>
	{
		protected override string GetEntityName() => "Player";

		protected override dynamic EntityApi => _apiClient.Api.Players;

		protected override int? GetSelectItemId()
		{
			return _selectedItem?.PlayerId;
		}

		protected override PlayerFormModel MapSelectedItemToFormModel()
		{
			return new PlayerFormModel
			{
				FirstName = _selectedItem?.FirstName ?? string.Empty,
				LastName = _selectedItem?.LastName ?? string.Empty,
				JerseyNumber = _selectedItem?.JerseyNumber,
				Position = Enum.TryParse<Position>(_selectedItem?.Position, out var pos) ? pos : (Position?)null,
				TeamID = _selectedItem?.TeamId,
				AssociationID = _selectedItem?.AssociationId
			};
		}

		protected override Player MapToEntityForAdd()
		{
			return new Player
			{
				FirstName = _formModel?.FirstName ?? string.Empty,
				LastName = _formModel?.LastName ?? string.Empty,
				Email = _formModel?.Email,
				BirthDate = ToDateTime(_formModel?.BirthDate),
				JerseyNumber = _formModel?.JerseyNumber,
				Position = _formModel?.Position,
				TeamID = _formModel?.TeamID,
				AssociationID = _formModel?.AssociationID
			};
		}

		protected override Player MapToEntityForUpdate()
		{
			return new Player
			{
				PlayerID = _selectedId,
				FirstName = _formModel?.FirstName ?? string.Empty,
				LastName = _formModel?.LastName ?? string.Empty,
				Email = _formModel?.Email,
				BirthDate = ToDateTime(_formModel?.BirthDate),
				JerseyNumber = _formModel?.JerseyNumber,
				Position = _formModel?.Position,
				TeamID = _formModel?.TeamID,
				AssociationID = _formModel?.AssociationID
			};
		}
	}
}
