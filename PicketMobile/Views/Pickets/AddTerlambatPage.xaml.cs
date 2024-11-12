using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

using PicketMobile.Models;

namespace PicketMobile.Views.Pickets;

public partial class AddTerlambatPage : ContentPage
{
    public AddTerlambatPage()
    {
        InitializeComponent();
        BindingContext = new AddTerlabatPageViewModel();
    }
}

internal class AddTerlabatPageViewModel : BaseNotify
{
    private StudentToLateModel model;

    public StudentToLateModel Model
    {
        get { return model; }
        set { SetProperty(ref model, value); }
    }




    private ICommand _searchCommand;

    public ICommand SearchCommand
    {
        get { return _searchCommand; }
        set
        {
            SetProperty(ref _searchCommand, value);
        }
    }


    private ICommand scanCommand;

    public ICommand ScanCommand
    {
        get { return scanCommand; }
        set { SetProperty(ref scanCommand, value); }
    }


    private ICommand addCommand;

    public ICommand AddCommand
    {
        get { return addCommand; }
        set { SetProperty(ref addCommand, value); }
    }



    private ICommand cancelCommand;

    public ICommand CancelCommand
    {
        get { return cancelCommand; }
        set { SetProperty(ref cancelCommand, value); }
    }



    private string searchText;

    public string SearchText
    {
        get { return searchText; }
        set { SetProperty(ref searchText, value); }
    }



    public AddTerlabatPageViewModel()
    {
        SearchCommand = new RelayCommand<object>(searchCommandAction, searchCommandValidation);
        ScanCommand = new RelayCommand(scanCommandAction);
        AddCommand = new RelayCommand(addCommandAction, addCommandValidation);
        CancelCommand = new RelayCommand(cancelCommandAction);
        this.PropertyChanged += (s, p) =>
        {
            if (p.PropertyName == "SearchText")
            {
                SearchCommand = new RelayCommand<object>(searchCommandAction, searchCommandValidation);
            }

            if (p.PropertyName == "Model")
            {
                AddCommand = new RelayCommand(addCommandAction, addCommandValidation);
            }
        };

    }

    private bool searchCommandValidation(object? obj)
    {
        if (string.IsNullOrEmpty(SearchText)||SearchText.Length <3)
            return false;
        return true;
    }

    private void searchCommandAction(object? obj)
    {
        try
        {

        }
        catch (Exception)
        {

            throw;
        }
    }

    private void scanCommandAction()
    {
        throw new NotImplementedException();
    }

    private void cancelCommandAction()
    {
        Shell.Current.Navigation.PopModalAsync();
    }

    private void addCommandAction()
    {
        throw new NotImplementedException();
    }

    private bool addCommandValidation()
    {
        if (Model == null || Model.CreateAt.Hour <= 8 || string.IsNullOrEmpty(Model.Description))
            return false;
        return true;
    }

 

}