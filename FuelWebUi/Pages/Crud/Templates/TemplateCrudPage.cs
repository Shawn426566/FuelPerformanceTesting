using FuelWebUi.ApiClient;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace FuelWebUi.Pages.Crud.Templates
{
    public abstract class TemplateCrudPage<TDto, TFormModel, TEntity> : ComponentBase
    {
        protected List<TDto> _items { get; set; } = [];
        protected TFormModel? _formModel { get; set; }
        protected string? _successMessage { get; set; }
        protected string? _errorMessage { get; set; }
        protected bool _isLoading { get; set; }
        protected bool _isSaving { get; set; }
        protected int _currentPage { get; set; } = 1;
        protected int _pageSize { get; set; } = 30;
        protected int? _totalCount { get; set; }
        protected int _totalPages => (_totalCount > 0 && _pageSize > 0)
            ? (int)Math.Ceiling(_totalCount.Value / (double)_pageSize)
            : 0;
        protected bool _isPageLoading { get; set; } = false;
        protected TDto? _selectedItem { get; set; }
        protected bool _showForm { get; set; }
        protected bool _isEditMode { get; set; }
        protected int? _selectedId { get; set; }

        [Inject]
        protected FuelApiClient _apiClient { get; set; } = default!;

        [Inject]
        protected NavigationManager _navigationManager { get; set; } = default!;

        [Inject]
        protected ILogger<TemplateCrudPage<TDto, TFormModel, TEntity>> _logger { get; set; } = default!;

        protected virtual async Task NextPageAsync()
        {
            if (_currentPage < _totalPages && !_isPageLoading)
            {
                _isPageLoading = true;
                _currentPage++;
                await LoadItemsAsync();
                _isPageLoading = false;
            }
        }

        protected virtual async Task PreviousPageAsync()
        {
            if (_currentPage > 1 && !_isPageLoading)
            {
                _isPageLoading = true;
                _currentPage--;
                await LoadItemsAsync();
                _isPageLoading = false;
            }
        }

        protected virtual void ResetPagination()
        {
            _currentPage = 1;
            _totalCount = 0;
        }

        protected virtual void ShowAddForm()
        {
            _isEditMode = false;
            _selectedId = null;
            _formModel = CreateNewFormModel();
            _errorMessage = null;
            _successMessage = null;
            _showForm = true;
        }

        protected virtual void ShowEditForm(TDto item)
        {
            _selectedItem = item;
            _isEditMode = true;
            _errorMessage = null;
            _successMessage = null;
            try{
                if (_selectedItem == null)
                    throw new InvalidOperationException("No item selected for editing.");
                _selectedId = GetSelectItemId();
                if (_selectedId == null)
                    throw new InvalidOperationException("Selected item ID is null.");
                _formModel = MapSelectedItemToFormModel();
                _showForm = true;
            }
            catch (InvalidOperationException ex)
            {
                _errorMessage = ex.Message;
                _logger.LogError(ex, "Edit form could not be shown due to invalid operation.");
                _ = FadeOutMessageAsync();
            }
        }

        protected virtual async Task OnValidSubmitAsync()
        {
            if (_formModel == null)
            {
                _errorMessage = "FormModel is Null.";
                _logger.LogError("FormModel is Null.");
                _ = FadeOutMessageAsync();
                return;
            }
            if (_isEditMode)
            {
                await UpdateAsync();
            }
            else
            {
                await AddAsync();
            }
        }

        protected virtual async Task DeleteItemAsync()
        {
            if (_selectedItem != null)
            {
                await DeleteAsync();
            }
            else
            {
                _errorMessage = "No item selected for deletion.";
                _logger.LogError("No item selected for deletion.");
                _ = FadeOutMessageAsync();
            }
        }

        protected virtual void HideForm()
        {
            _showForm = false;
            _isEditMode = false;
            _selectedId = null;
        }

        protected async Task FadeOutMessageAsync()
        {
            await Task.Delay(3000);
            _successMessage = null;
            _errorMessage = null;
            StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadItemsAsync();
        }

        protected async Task LoadItemsAsync()
        {
            try
            {
                _errorMessage = null;
                _logger.LogInformation("Loading {EntityName} - Page {CurrentPage} - PageSize {PageSize}", GetEntityName(), _currentPage, _pageSize);
                var response = await EntityApi.GetAsync((Action<dynamic>)(config =>
                {
                    config.QueryParameters.SortBy = "name";
                    config.QueryParameters.SortDir = "asc";
                    config.QueryParameters.Page = _currentPage;
                    config.QueryParameters.PageSize = _pageSize;
                }));
                _items = response?.Items ?? new List<TDto>();
                _totalCount = response?.TotalCount ?? 0;
            }
            catch (Exception ex)
            {
                _errorMessage = $"Failed to load {GetEntityName()}: {ex.Message}";
                _logger.LogError(ex, "Error occurred while loading {EntityName}s", GetEntityName());
            }
            finally
            {
                _isLoading = false;
            }
            
        }
    
        protected async Task AddAsync()
        {
            _isSaving = true;
            _errorMessage = null;
            _successMessage = null;
            try
            {
                if (_formModel == null)
                    throw new InvalidOperationException("Form is null.");
                var request = MapToEntityForAdd();
                await EntityApi.PostAsync(request);
                _successMessage = $"{GetEntityName()} added successfully.";
                _ = FadeOutMessageAsync();
                HideForm();
                await LoadItemsAsync();
            }
            catch (Exception ex)
            {
                _errorMessage = $"Failed to add {GetEntityName()}: {ex.Message}";
                _logger.LogError(ex, "Failed to add {EntityName}", GetEntityName());
                _ = FadeOutMessageAsync();
            }
            finally
            {
                _isSaving = false;
            }
        }
        protected async Task UpdateAsync()
        {
            _isSaving = true;
            _errorMessage = null;
            _successMessage = null;
            try
            {   
                if (_formModel == null)
                    throw new InvalidOperationException("Form is null.");
                var request = MapToEntityForUpdate();
                if (_selectedId == null)
                    throw new InvalidOperationException("Selected item is null.");
                await EntityApi[_selectedId.Value].PutAsync(request);
                _successMessage = $"{GetEntityName()} updated successfully.";
                _ = FadeOutMessageAsync();
                HideForm();
                await LoadItemsAsync();
            }
            catch (Exception ex)
            {
                _errorMessage = $"Failed to update {GetEntityName()}: {ex.Message}";
                _logger.LogError(ex, "Failed to update {EntityName}", GetEntityName());
                _ = FadeOutMessageAsync();
            }
            finally
            {
                _isSaving = false;
            }
        }
        protected async Task DeleteAsync()
        {
            _isSaving = true;
            _errorMessage = null;
            _successMessage = null;
            try
            {
                if (_selectedId == null)
                    throw new InvalidOperationException("No item selected for deletion.");
                await EntityApi[_selectedId.Value].DeleteAsync();
                _successMessage = $"{GetEntityName()} deleted successfully.";
                _ = FadeOutMessageAsync();
                HideForm();
                await LoadItemsAsync();
            }
            catch (Exception ex)
            {
                _errorMessage = $"Failed to delete {GetEntityName()}: {ex.Message}";
                _logger.LogError(ex, "Failed to delete {EntityName}", GetEntityName());
                _ = FadeOutMessageAsync();
            }
            finally
            {
                _isSaving = false;
            }
        }

        protected static DateTime? ToDateTime(object? date)
        {
            if (date == null) return null;
            if (date is DateTimeOffset dto)
                return dto.UtcDateTime;
            if (date is DateTime dt)
                return dt;
            if (date is Microsoft.Kiota.Abstractions.Date kiotaDate)
            {
                if (DateTime.TryParse(kiotaDate.ToString(), out var parsed))
                    return parsed;
            }
            return null;
        }

        protected virtual TFormModel CreateNewFormModel() => Activator.CreateInstance<TFormModel>();

        protected abstract string GetEntityName();
        protected abstract dynamic EntityApi { get; }
        protected abstract int? GetSelectItemId();
        protected abstract TFormModel MapSelectedItemToFormModel();
        protected abstract TEntity MapToEntityForAdd();
        protected abstract TEntity MapToEntityForUpdate();
    }
}
