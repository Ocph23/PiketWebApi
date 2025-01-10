using CommunityToolkit.Mvvm.Messaging;
using PicketMobile.Models;
using SharedModel.Responses;
using System.Collections.ObjectModel;
using The49.Maui.BottomSheet;

namespace PicketMobile.Views.BottomSheets;

public partial class BrowseStudentBottomSheet : BottomSheet
{
    public ObservableCollection<StudentResponse> Datas { get; set; }


    private StudentResponse selectedStudent;

    public StudentResponse SelectedStudent
    {
        get { return selectedStudent; }
        set
        {
            selectedStudent = value;
            OnPropertyChanged("SelectedStudent");
        }
    }


    public BrowseStudentBottomSheet(IEnumerable<StudentResponse> students)
    {
        InitializeComponent();
        Datas = new ObservableCollection<StudentResponse>(students);
        BindingContext = this;
    }

    private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SelectedStudent = e.CurrentSelection[0] as StudentResponse;
        if (SelectedStudent != null)
        {
            WeakReferenceMessenger.Default.Send(new StudentSearchChangeMessage(SelectedStudent));
            this.DismissAsync();
        }
    }
}