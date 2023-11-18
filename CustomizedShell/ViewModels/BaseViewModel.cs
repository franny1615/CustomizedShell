using CommunityToolkit.Mvvm.ComponentModel;
using CustomizedShell.Services;

namespace CustomizedShell.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    internal LanguageService Lang => LanguageService.Instance;
}
