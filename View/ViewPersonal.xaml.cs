using Module07DataAccess.Services;
using Module07DataAccess.ViewModel;
using MySql.Data.MySqlClient;

namespace Module07DataAccess.View;

public partial class ViewPersonal : ContentPage
{
	public ViewPersonal()
	{
		InitializeComponent();

		var personalViewModel = new PersonalViewModel();
		BindingContext = personalViewModel;
	}
}