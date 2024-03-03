using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Inventory.Services.Interfaces;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Inventory.Pages;

public class SendFeedbackPage : BasePage
{
    #region Private Properties
    private SendFeedbackViewModel _viewModel => (SendFeedbackViewModel)BindingContext;
    private readonly ILanguageService _LangService;
    private readonly Grid _ContentLayout = new()
    {
        RowDefinitions = Rows.Define(Auto, Star, Auto),
        RowSpacing = 16,
        Padding = 16
    };
    private readonly MaterialEntry _Subject;
    private readonly MaterialEntry _Body;
    private readonly FloatingActionButton _SendFeedback = new()
    {
        FABBackgroundColor = Application.Current.Resources["Primary"] as Color,
        TextColor = Colors.White,
        FABStyle = FloatingActionButtonStyle.Extended
    };
    #endregion

    #region Constructor
    public SendFeedbackPage(
        ILanguageService languageService,
        SendFeedbackViewModel feedbackVM) : base(languageService)
    {
        BindingContext = feedbackVM;
        _LangService = languageService;

        Title = languageService.StringForKey("Feedback");
        _Subject = new(feedbackVM.Subject);
        _Body = new(feedbackVM.Body);
        _Body.IsMultiLine = true;
        _SendFeedback.Text = languageService.StringForKey("Send Feedback");

        _ContentLayout.Add(_Subject.Row(0));
        _ContentLayout.Add(_Body.Row(1));
        _ContentLayout.Add(_SendFeedback.Row(2));

        Content = _ContentLayout;

        _SendFeedback.Clicked += Send;
    }
    ~SendFeedbackPage()
    {
        _SendFeedback.Clicked -= Send;
    }

    private async void Send(object sender, ClickedEventArgs e)
    {
        bool haveSubject = !string.IsNullOrEmpty(_viewModel.Subject.Text);
        bool haveBody = !string.IsNullOrEmpty(_viewModel.Body.Text);

        if (!haveSubject)
        {
            _Subject.ShowStatus(_LangService.StringForKey("Required"), MaterialIcon.Info, Colors.Red);
        }

        if (!haveBody)
        {
            _Body.ShowStatus(_LangService.StringForKey("Required"), MaterialIcon.Info, Colors.Red);
        }

        if (!haveSubject && !haveBody) 
        {
            return;
        }

        await _viewModel.Send();
        await Shell.Current.GoToAsync("..");
    }
    #endregion
}

public class SendFeedbackViewModel : ObservableObject
{
    private readonly IEmailService _emailService;

    public MaterialEntryModel Subject = new();
    public MaterialEntryModel Body = new();

    public SendFeedbackViewModel(
        ILanguageService languageService,
        IEmailService emailService)
    {
        _emailService = emailService;

        Subject.PlaceholderIcon = MaterialIcon.Abc;
        Body.PlaceholderIcon = MaterialIcon.Description;

        Subject.Placeholder = languageService.StringForKey("Subject");
        Body.Placeholder = languageService.StringForKey("Body");
    }

    public async Task<bool> Send()
    {
        return await _emailService.SendFeedback(Subject.Text, Body.Text);
    }
}