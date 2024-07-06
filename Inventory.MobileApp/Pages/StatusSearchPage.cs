using Inventory.MobileApp.Models;
using Inventory.MobileApp.Controls;
using Inventory.MobileApp.Services;
using Inventory.MobileApp.ViewModels;

namespace Inventory.MobileApp.Pages;

public class StatusSearchPage : BasePage
{
    private readonly StatusSearchViewModel _ViewModel;
    private readonly SearchView<Status> _Search;
    private bool _IsEditing = false;

    public StatusSearchPage(StatusSearchViewModel statusSearchViewModel)
    {
        _ViewModel = statusSearchViewModel;
        _Search = new(_ViewModel);
        _Search.SearchLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical) { ItemSpacing = 12 };
        _Search.CardTemplate = new DataTemplate(() =>
        {
            var view = new StatusCardView();
            view.SetBinding(BindingContextProperty, ".");
            view.SetBinding(StatusCardView.DescriptionProperty, "Description");
            view.Delete += DeleteStatus;
            view.Edit += UpdateStatus;

            return view;
        });
        _Search.AddItem += AddStatus;

        Title = LanguageService.Instance["Statuses"];
        Content = _Search;
    }

    private void AddStatus(object? sender, EventArgs e)
    {
        _IsEditing = true;
        Navigation.PushModalAsync(PageService.TakeUserInput(
            LanguageService.Instance["Enter the status description below."],
            "",
            Keyboard.Plain,
            submitted: async (status) =>
            {
                if (string.IsNullOrEmpty(status)) // canceled or entered empty text, don't do anything.
                    return;

                _Search.IsLoading = true;

                var response = await _ViewModel.InsertStatus(status);
                if (!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    _Search.IsLoading = false;
                    this.DisplayCommonError(response.ErrorMessage);
                    return;
                }

                _Search.TriggerRefresh();
                _Search.IsLoading = false; // fail safe

                _IsEditing = false;
            },
            canceled: () =>
            {
                _IsEditing = false;
            }));
    }

    private async void DeleteStatus(object? sender, EventArgs e)
    {
        if (sender is StatusCardView card && card.BindingContext is Status status) 
        {
            _Search.IsLoading = true;
            
            var response = await _ViewModel.DeleteStatus(status.Id);
            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                _Search.IsLoading = false;
                this.DisplayCommonError(response.ErrorMessage);
                return;
            }

            switch (response.Data)
            {
                case DeleteResult.SuccesfullyDeleted:
                    _Search.TriggerRefresh();
                    break;
                case DeleteResult.LinkedToOtherItems:
                    await DisplayAlert(
                        LanguageService.Instance["In Use"],
                        $"{status.Description} - {LanguageService.Instance["is in use in at least one inventory item"]}.",
                        LanguageService.Instance["OK"]);
                    break;
            }

            _Search.IsLoading = false; // fail safe
        }
    }

    private void UpdateStatus(object? sender, EventArgs e)
    {
        if (sender is StatusCardView card && card.BindingContext is Status status)
        {
            _IsEditing = true;
            Navigation.PushModalAsync(PageService.TakeUserInput(
                LanguageService.Instance["Enter the status description below."],
                status.Description,
                Keyboard.Plain,
                submitted: async (statusStr) =>
                {
                    if (string.IsNullOrEmpty(statusStr)) // canceled or entered empty text, don't do anything.
                        return;

                    _Search.IsLoading = true;

                    status.Description = statusStr;
                    var response = await _ViewModel.UpdateStatus(status);
                    if (!string.IsNullOrEmpty(response.ErrorMessage))
                    {
                        _Search.IsLoading = false;
                        this.DisplayCommonError(response.ErrorMessage);
                        return;
                    }

                    _Search.TriggerRefresh();
                    _Search.IsLoading = false; // fail safe

                    _IsEditing = false;
                },
                canceled: () =>
                {
                    _IsEditing = false;
                }));
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!_IsEditing)
        {
            _Search.TriggerRefresh();
        }
    }
}