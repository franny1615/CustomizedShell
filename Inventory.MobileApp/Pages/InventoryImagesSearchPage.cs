using CommunityToolkit.Maui.Storage;
using Inventory.MobileApp.Controls;
using Inventory.MobileApp.Models;
using Inventory.MobileApp.Services;
using Inventory.MobileApp.ViewModels;

namespace Inventory.MobileApp.Pages;

public class InventoryImagesSearchPage : BasePage
{
    private readonly InventoryImageSearchViewModel _ViewModel;
    private readonly SearchView<InventoryImage> _Search;
    private bool _IsEditing = false;

    public InventoryImagesSearchPage(InventoryImageSearchViewModel vm)
    {
        _ViewModel = vm;
        _Search = new(vm);
        _Search.ShowSearch = false;
        _Search.CanAddItems = PermsUtils.IsAllowed(InventoryPermissions.CanAddInventory);
        _Search.SearchLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical) { ItemSpacing = 12 };
        _Search.CardTemplate = new DataTemplate(() =>
        {
            var view = new InventoryImageCardView();
            view.SetBinding(InventoryImageCardView.BindingContextProperty, ".");
            view.SetBinding(InventoryImageCardView.ImageBase64Property, "ImageBase64");
            view.EditImage += EditImage;
            view.DownloadImage += DownloadImage;
            view.DeleteImage += DeleteImage;

            return view;
        });
        _Search.AddItem += AddImage;

        Title = LanguageService.Instance["Inventory Images"];
        Content = _Search;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!_IsEditing)
        {
            _Search.TriggerRefresh();
        }
    }

    private async void DeleteImage(object? sender, EventArgs e)
    {
        if (sender is InventoryImageCardView card && card.BindingContext is InventoryImage img)
        {
            _Search.IsLoading = true;

            var response = await _ViewModel.DeleteImage(img.Id);
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
                case DeleteResult.LinkedToOtherItems: // shouldn't happen but in here for completeness.
                    break;
            }

            _Search.IsLoading = false; // fail safe
        }
    }

    private async void DownloadImage(object? sender, EventArgs e)
    {
        if (sender is InventoryImageCardView card && card.BindingContext is InventoryImage img)
        {
            _Search.IsLoading = true;
            try
            {
                byte[] bytes = Convert.FromBase64String(img.ImageBase64);
                using var stream = new MemoryStream(bytes);
                var saveresult = await FileSaver.Default.SaveAsync($"{Guid.NewGuid()}.png", stream);
            }
            catch { }
            _Search.IsLoading = false;
        }
    }

    private async void EditImage(object? sender, EventArgs e)
    {
        if (sender is InventoryImageCardView card && card.BindingContext is InventoryImage img)
        {
            _IsEditing = true;
            _Search.IsLoading = true;
            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    FileResult? photo = await MediaPicker.Default.CapturePhotoAsync();
                    if (photo != null)
                    {
                        using var stream = await photo.OpenReadAsync();
                        using var memstream = new MemoryStream();
                        stream.CopyTo(memstream);
                        string base64 = Convert.ToBase64String(memstream.ToArray());

                        var response = await _ViewModel.UpdateImage(new InventoryImage
                        {
                            Id = img.Id,
                            InventoryId = _ViewModel.InventoryItem.Id,
                            ImageBase64 = base64,
                        });

                        if (!string.IsNullOrEmpty(response.ErrorMessage))
                            this.DisplayCommonError(response.ErrorMessage);
                    }
                }
            }
            catch { }
            _Search.IsLoading = false;
            _IsEditing = false;
            _Search.TriggerRefresh();
        }
    }

    private async void AddImage(object? sender, EventArgs e)
    {
        _IsEditing = true;
        _Search.IsLoading = true;
        try
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                FileResult? photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo != null)
                {
                    using var stream = await photo.OpenReadAsync();
                    using var memstream = new MemoryStream();
                    stream.CopyTo(memstream);
                    string base64 = Convert.ToBase64String(memstream.ToArray());

                    var response = await _ViewModel.InsertImage(new InventoryImage
                    {
                        InventoryId = _ViewModel.InventoryItem.Id,
                        ImageBase64 = base64,
                    });

                    if (!string.IsNullOrEmpty(response.ErrorMessage))
                        this.DisplayCommonError(response.ErrorMessage);                        
                }
            }
        }
        catch { }
        _IsEditing = false;
        _Search.IsLoading = false;
        _Search.TriggerRefresh();
    }
}
