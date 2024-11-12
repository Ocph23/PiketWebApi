using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace PicketMobile.Views.Pickets;

public partial class AddTerlambatPage : ContentPage
{
	public AddTerlambatPage()
	{
		InitializeComponent();
		BindingContext = new AddTerlambatPageViewModel();
	}
}

internal class AddTerlambatPageViewModel:BaseNotify
{
	private Models.StudentToLateModel studentToLateModel  = new Models.StudentToLateModel();

	public Models.StudentToLateModel Model
	{
		get { return studentToLateModel; }
		set { studentToLateModel = value; }
	}


	private ICommand addCommand;

	public ICommand AddCommand
	{
		get { return addCommand; }
		set { SetProperty(ref addCommand , value); }
	}


	public AddTerlambatPageViewModel()
    {

		AddCommand = new RelayCommand<object>(AddAcommandAcation, AddCommandValidate);
    }

    private void AddAcommandAcation(object? obj)
    {
		try
		{

		}
		catch (Exception ex)
		{
			Shell.Current.DisplayAlert("Error",ex.Message,"OK");
		}
    }

    private bool AddCommandValidate(object? obj)
    {
        if(Model.Student==null || (Model.AtTime < new TimeSpan(8,0,0) ))
		{
			return false;
		}

		return true;
    }
}