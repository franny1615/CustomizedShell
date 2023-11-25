// using CommunityToolkit.Maui.Behaviors;
// using CommunityToolkit.Maui.Markup;
// using Maui.Components.Controls;
// using Maui.Components.Enums;
// using Maui.Components.Interfaces;

// namespace Maui.Components.Pages;

// public class EditSearchablePopup : PopupPage
// {
//     #region Private Properties
//     private readonly EditSearchableArgs _Args;
//     private readonly ILanguageService _LanguageService;
//     private readonly ISearchViewModel _SearchViewModel;
//     private readonly ISearchable _Searchable;
//     private readonly VerticalStackLayout _ContentLayout = new()
//     {
//         Spacing = 8,
//     };
//     private readonly Label _TitleLabel = new()
//     {
//         FontSize = 21,
//         FontAttributes = FontAttributes.Bold,
//         HorizontalTextAlignment = TextAlignment.Start,
//         VerticalOptions = LayoutOptions.Center,
//         MaxLines = 1
//     };
//     private readonly Image _CloseIcon = new()
//     {
//         HeightRequest = 30,
//         WidthRequest = 30,
//     };
//     private readonly StyledEntry _NameEntry = new();
//     private readonly StyledEntry _DescriptionEntry = new();
//     private readonly FloatingActionButton _SaveButton = new();
//     private readonly FloatingActionButton _DeleteButton = new();
//     private readonly IconTintColorBehavior _CloseIconTint = new();
//     #endregion

//     #region Constructor
//     public EditSearchablePopup(
//         ILanguageService languageService,
//         ISearchable searchable,
//         ISearchViewModel searchViewModel,
//         CardStyle cardStyle,
//         bool isNew = false) : base(languageService)
//     {
//         _LanguageService = languageService;
//         _Searchable = searchable;
//         _SearchViewModel = searchViewModel;
        
//         _CloseIconTint.SetAppThemeColor(IconTintColorBehavior.TintColorProperty, Colors.Black, Colors.White);
//         _CloseIcon.Behaviors.Add(_CloseIconTint);

//         _CloseIcon.TapGesture(async () => 
//         {
//             await this.Navigation.PopModalAsync();
//         });

//         _CloseIcon.Source = args.CloseIcon;
//         _TitleLabel.Text = args.Title;

//         _NameEntry.Placeholder = args.NamePlaceholder;
//         _DescriptionEntry.Placeholder = args.DescriptionPlaceholder;

//         _NameEntry.Text = searchable.Name;
//         _DescriptionEntry.Text = searchable.Description;

//         _SaveButton.Text = args.SavePlaceholder;
//         _SaveButton.ImageSource = args.SaveIcon;

//         _DeleteButton.Text = args.DeletePlaceholder;
//         _DeleteButton.ImageSource = args.DeleteIcon;

//         _ContentLayout.Children.Add(new Grid 
//         {
//             Children = 
//             {
//                 _TitleLabel.Start(),
//                 _CloseIcon.End()
//             }
//         });
//         switch (cardStyle)
//         {
//             case CardStyle.Mini:
//                 _ContentLayout.Children.Add(_NameEntry);
//                 break;
//             case CardStyle.Regular:
//                 _ContentLayout.Children.Add(_NameEntry);
//                 _ContentLayout.Children.Add(_DescriptionEntry);
//                 break;
//         }
//         _ContentLayout.Children.Add(_SaveButton);
        
//         if (!isNew)
//         { 
//             _ContentLayout.Children.Add(_DeleteButton);
//         }
        
//         _SaveButton.FABStyle = FloatingActionButtonStyle.Extended;
//         _SaveButton.FABBackgroundColor = Colors.Green;
//         _SaveButton.TextColor = Colors.White;
//         _DeleteButton.FABStyle = FloatingActionButtonStyle.Extended;
//         _DeleteButton.TextColor = Colors.White;
//         _DeleteButton.FABBackgroundColor = Colors.Red;

//         PopupContent = _ContentLayout;
//         PopupStyle = PopupStyle.Center;
//     }
//     #endregion

//     #region Overrides
//     protected override void OnAppearing()
//     {
//         base.OnAppearing();
//         _SaveButton.Clicked += Save;
//         _DeleteButton.Clicked += Delete;
//     }

//     protected override void OnDisappearing()
//     {
//         _SaveButton.Clicked -= Save;
//         _DeleteButton.Clicked -= Delete;
//         base.OnDisappearing();
//     }
//     #endregion

//     #region Helpers

//     private async void Delete(object sender, ClickedEventArgs e)
//     {
//         bool delete = true;
//         if (_Args.HasDeleteConfirmation)
//         {
//             delete = await Application.Current.MainPage.DisplayAlert(
//                 _Args.DeleteConfirmationTitle,
//                 _Args.DeleteConfirmationMessage,
//                 _Args.ConfirmDelete,
//                 _Args.DenyDelete);
//         }

//         if (!delete)
//         {
//             return;
//         }

//         bool wasAbleToDelete = await _SearchViewModel.Delete(_Searchable);
//         if (!wasAbleToDelete)
//         {
//             await Application.Current.MainPage.DisplayAlert(
//                 _Args.DeleteErrorTitle,
//                 _Args.DeleteErrorMessage,
//                 _Args.DeleteErrorAcknowledgement);
//         }
//         else
//         {
//             await this.Navigation.PopModalAsync();
//         }
//     }

//     private async void Save(object sender, ClickedEventArgs e)
//     {
//         bool save = true;
//         if (_Args.HasSaveConfirmation)
//         {
//             save = await Application.Current.MainPage.DisplayAlert(
//                 _Args.SaveConfirmationTitle,
//                 _Args.SaveConfirmationMessage,
//                 _Args.ConfirmSave,
//                 _Args.DenySave);
//         }

//         if (!save)
//         {
//             return;
//         }

//         _Searchable.Name = _NameEntry.Text;
//         _Searchable.Description = _DescriptionEntry.Text;

//         bool wasAbleToSave = await _SearchViewModel.Save(_Searchable);
//         if (!wasAbleToSave)
//         {
//             await Application.Current.MainPage.DisplayAlert(
//                 _Args.SaveErrorTitle,
//                 _Args.SaveErrorMessage,
//                 _Args.SaveErrorAcknowledgement);
//         }
//         else
//         {
//             await this.Navigation.PopModalAsync();
//         }
//     }
//     #endregion
// }
