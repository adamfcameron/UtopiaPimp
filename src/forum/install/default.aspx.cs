/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2009 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI.WebControls;
using YAF.Classes;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Install
{
	/// <summary>
	/// Summary description for install.
	/// </summary>
	public partial class _default : System.Web.UI.Page
	{
		protected int _dbVersionBeforeUpgrade;
		private string _loadMessage = "";

		private const string _bbcodeImport = "bbCodeExtensions.xml";
		private const string _fileImport = "fileExtensions.xml";

		#region events
		private void Page_Load( object sender, System.EventArgs e )
		{
			if ( !IsPostBack )
			{
				Cache ["DBVersion"] = DB.DBVersion;

				InstallWizard.ActiveStepIndex = IsInstalled ? 1 : 0;
				
				if ( !IsInstalled )
				{
					// fake the board settings
					YafContext.Current.BoardSettings = new YafBoardSettings();
				}

				TimeZones.DataSource = YafStaticData.TimeZones( "english.xml" );

				DataBind();

				TimeZones.Items.FindByValue( "0" ).Selected = true;
			}
		}

		protected void UserChoice_SelectedIndexChanged( object sender, EventArgs e )
		{
			if ( UserChoice.SelectedValue == "create" )
			{
				ExistingUserHolder.Visible = false;
				CreateAdminUserHolder.Visible = true;
			}
			else if ( UserChoice.SelectedValue == "existing" )
			{
				ExistingUserHolder.Visible = true;
				CreateAdminUserHolder.Visible = false;
			}
		}

		protected void Password_Postback(object sender, System.EventArgs e)
		{
			// prepare even arguments for calling Wizard_NextButtonClick event handler
			WizardNavigationEventArgs eventArgs = new WizardNavigationEventArgs(InstallWizard.ActiveStepIndex, InstallWizard.ActiveStepIndex + 1);

			// call it
			Wizard_NextButtonClick(sender, eventArgs);

			// move to next step only if it wasn't cancelled within next button event handler
			if (!eventArgs.Cancel) InstallWizard.MoveTo(InstallWizard.WizardSteps[eventArgs.NextStepIndex]);
		}

		protected void Wizard_FinishButtonClick( object sender, WizardNavigationEventArgs e )
		{
			if ( YAF.Classes.Config.IsDotNetNuke )
			{
				//Redirect back to the portal main page.
				string rPath = YafForumInfo.ForumRoot;
				int pos = rPath.IndexOf( "/", 2 );
				rPath = rPath.Substring( 0, pos );
				Response.Redirect( rPath );
			}
			else
				Response.Redirect( "~/" );
		}

		protected void Wizard_PreviousButtonClick( object sender, WizardNavigationEventArgs e )
		{
			// go back only from last step (to user/roles migration)
			if ( e.CurrentStepIndex == ( InstallWizard.WizardSteps.Count - 1 ) )
				InstallWizard.MoveTo( InstallWizard.WizardSteps [e.CurrentStepIndex - 1] );
			else
				// othwerise cancel action
				e.Cancel = true;
		}

		protected void Wizard_ActiveStepChanged( object sender, EventArgs e )
		{
			if ( InstallWizard.ActiveStepIndex == 1 && !IsInstalled )
			{
				InstallWizard.ActiveStepIndex++;
			}
			else if ( InstallWizard.ActiveStepIndex == 3 && DB.IsForumInstalled )
			{
				InstallWizard.ActiveStepIndex++;
			}
			else if ( InstallWizard.ActiveStepIndex == 4 )
			{
				if ( !IsInstalled )
				{
					// no migration because it's a new install...
					InstallWizard.ActiveStepIndex++;
				}
				else
				{
					object version = Cache["DBVersion"];

					if ( version == null ) version = DB.DBVersion;

					if (((int)version) >= 30 || ((int)version) == -1 )
					{
						// migration is NOT needed...
						InstallWizard.ActiveStepIndex++;
					}

					Cache.Remove("DBVersion");
				}
			}
		}

		protected void Wizard_NextButtonClick( object sender, WizardNavigationEventArgs e )
		{
			e.Cancel = true;
			//try
			{
				switch ( e.CurrentStepIndex )
				{
					case 0:
						if ( TextBox1.Text == string.Empty )
						{
							AddLoadMessage( "Missing configuration password." );
							return;
						}
						else if ( TextBox2.Text != TextBox1.Text )
						{
							AddLoadMessage( "Password not verified." );
							return;
						}

						try
						{
							Configuration config = WebConfigurationManager.OpenWebConfiguration( "~/" );
							AppSettingsSection appSettings = config.GetSection( "appSettings" ) as AppSettingsSection;

							if ( appSettings != null )
							{
								if ( appSettings.Settings ["YAF.ConfigPassword"] != null )
								{
									appSettings.Settings.Remove( "YAF.ConfigPassword" );
								}

								appSettings.Settings.Add( "YAF.ConfigPassword", TextBox1.Text );
							}

							config.Save( ConfigurationSaveMode.Modified );
							e.Cancel = false;
						}
						catch
						{
							// just a warning now...
							throw new Exception( "Cannot save the YAF.ConfigPassword to the app.config file. Please verify that the ASPNET user has write access permissions to the app.config file. Or modify the app.config \"YAF.ConfigPassword\" key with a plaintext password and try again." );
						}					

						break;
					case 1:
						if ( Config.GetConfigValueAsString( "YAF.ConfigPassword" ) == System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile( TextBox3.Text, "md5" ) ||
									Config.GetConfigValueAsString( "YAF.ConfigPassword" ) == TextBox3.Text.Trim() )
							e.Cancel = false;
						else
							AddLoadMessage( "Wrong password!" );
						break;
					case 2:
						if ( UpgradeDatabase( FullTextSupport.Checked ) )
							e.Cancel = false;
						break;
					case 3:
						if ( CreateForum() )
							e.Cancel = false;
						break;
					case 4:
						// migrate users/roles only if user does not want to skip
						if ( !skipMigration.Checked )
						{
							RoleMembershipHelper.SyncRoles( PageBoardID );
							RoleMembershipHelper.SyncUsers( PageBoardID );
						}
						e.Cancel = false;
						break;
					case 5:
						YafContext.Current.BoardSettings = null;
						break;
					default:
						throw new ApplicationException( e.CurrentStepIndex.ToString() );
				}
			}
			/*catch ( Exception x )
			{
				AddLoadMessage( x.Message );
			}*/
		}
		#endregion

		protected override void Render( System.Web.UI.HtmlTextWriter writer )
		{
			base.Render( writer );
			if ( _loadMessage != "" )
			{
				writer.WriteLine( "<script language='javascript'>" );
				writer.WriteLine( "onload = function() {" );
				writer.WriteLine( "	alert('{0}');", _loadMessage );
				writer.WriteLine( "}" );
				writer.WriteLine( "</script>" );
			}
		}

		void AddLoadMessage( string msg )
		{
			msg = msg.Replace( "\\", "\\\\" );
			msg = msg.Replace( "'", "\\'" );
			msg = msg.Replace( "\r\n", "\\r\\n" );
			msg = msg.Replace( "\n", "\\n" );
			msg = msg.Replace( "\"", "\\\"" );
			_loadMessage += msg + "\\n\\n";
		}

		#region property IsInstalled
		private bool IsInstalled
		{
			get
			{
				return !String.IsNullOrEmpty( Config.GetConfigValueAsString( "YAF.configPassword" ) );
			}
		}
		#endregion

		#region method UpgradeDatabase
		bool UpgradeDatabase(bool fullText)
		{
			//try
			{
				FixAccess( false );

				foreach ( string script in DB.ScriptList )
				{
					ExecuteScript( script, true );
				}

				FixAccess( true );

				int prevVersion = DB.DBVersion;

				DB.system_updateversion(YafForumInfo.AppVersion, YafForumInfo.AppVersionName);

				// Ederon : 9/7/2007
				// resync all boards - necessary for propr last post bubbling
				DB.board_resync();

				// upgrade providers...
				YAF.Providers.Membership.DB.UpgradeMembership( prevVersion, YafForumInfo.AppVersion );

				if ( DB.IsForumInstalled && prevVersion < 30 )
				{
					// load default bbcode if available...
					if ( File.Exists( Request.MapPath( _bbcodeImport ) ) )
					{
						// import into board...
						using ( StreamReader bbcodeStream = new StreamReader( Request.MapPath( _bbcodeImport ) ) )
						{
							YAF.Classes.Data.Import.DataImport.BBCodeExtensionImport( PageBoardID, bbcodeStream.BaseStream );
							bbcodeStream.Close();
						}
					}

					// load default extensions if available...
					if ( File.Exists( Request.MapPath( _fileImport ) ) )
					{
						// import into board...
						using ( StreamReader fileExtStream = new StreamReader( Request.MapPath( _fileImport ) ) )
						{
							YAF.Classes.Data.Import.DataImport.FileExtensionImport( PageBoardID, fileExtStream.BaseStream );
							fileExtStream.Close();
						}
					}
				}

                //vzrus: uncomment it to not keep install/upgrade objects in DB and for better security 
                //DB.system_deleteinstallobjects();
			}
			/*catch ( Exception x )
			{
				AddLoadMessage( x.Message );
				return false;
			}*/

			// attempt to apply fulltext support if desired
			if ( fullText && DB.FullTextSupported )
			{
				try
				{
					ExecuteScript( DB.FullTextScript, false );
				}
				catch ( Exception x )
				{
					// just a warning...
					AddLoadMessage( "Warning: FullText Support wasn't installed: " + x.Message );
				}
			}

			return true;
		}
		#endregion

		#region method CreateForum
		private bool CreateForum()
		{
			if ( DB.IsForumInstalled )
			{
				AddLoadMessage( "Forum is already installed." );
				return false;
			}
			if ( TheForumName.Text.Length == 0 )
			{
				AddLoadMessage( "You must enter a forum name." );
				return false;
			}
			if ( ForumEmailAddress.Text.Length == 0 )
			{
				AddLoadMessage( "You must enter a forum email address." );
				return false;
			}

			MembershipUser user = null;

			if ( UserChoice.SelectedValue == "create" )
			{
				if ( UserName.Text.Length == 0 )
				{
					AddLoadMessage( "You must enter the admin user name," );
					return false;
				}
				if ( AdminEmail.Text.Length == 0 )
				{
					AddLoadMessage( "You must enter the administrators email address." );
					return false;
				}
				if ( Password1.Text.Length == 0 )
				{
					AddLoadMessage( "You must enter a password." );
					return false;
				}
				if ( Password1.Text != Password2.Text )
				{
					AddLoadMessage( "The passwords must match." );
					return false;
				}

				// create the admin user...
				MembershipCreateStatus status;
				user = Membership.CreateUser( UserName.Text, Password1.Text, AdminEmail.Text, SecurityQuestion.Text, SecurityAnswer.Text, true, out status );
				if ( status != MembershipCreateStatus.Success )
				{
					AddLoadMessage( string.Format( "Create Admin User Failed: {0}", GetMembershipErrorMessage( status ) ) );
					return false;
				}
			}
			else
			{
				// try to get data for the existing user...
				user = Membership.GetUser( ExistingUserName.Text.Trim() );

				if ( user == null )
				{
					AddLoadMessage( "Existing user name is invalid and does not represent a current user in the membership store." );
					return false;
				}
			}

			try
			{
				// add administrators and registered if they don't already exist...
				if ( !Roles.RoleExists( "Administrators" ) )
				{
					Roles.CreateRole( "Administrators" );
				}
				if ( !Roles.RoleExists( "Registered" ) )
				{
					Roles.CreateRole( "Registered" );
				}
				if ( !Roles.IsUserInRole( user.UserName, "Administrators" ) )
				{
					Roles.AddUserToRole( user.UserName, "Administrators" );
				}

				// logout administrator...
				FormsAuthentication.SignOut();
                YAF.Classes.Data.DB.system_initialize(TheForumName.Text, TimeZones.SelectedItem.Value, ForumEmailAddress.Text, "", user.UserName, user.Email, user.ProviderUserKey);
                YAF.Classes.Data.DB.system_updateversion(YafForumInfo.AppVersion, YafForumInfo.AppVersionName); DB.system_updateversion(YafForumInfo.AppVersion, YafForumInfo.AppVersionName);
                //vzrus: uncomment it to not keep install/upgrade objects in db for a place and better security
                //YAF.Classes.Data.DB.system_deleteinstallobjects();
				// load default bbcode if available...
				if ( File.Exists( Request.MapPath( _bbcodeImport ) ) )
				{
					// import into board...
					using ( StreamReader bbcodeStream = new StreamReader( Request.MapPath( _bbcodeImport ) ) )
					{
						YAF.Classes.Data.Import.DataImport.BBCodeExtensionImport( PageBoardID, bbcodeStream.BaseStream );
						bbcodeStream.Close();
					}
				}

				// load default extensions if available...
				if ( File.Exists( Request.MapPath( _fileImport ) ) )
				{
					// import into board...
					using ( StreamReader fileExtStream = new StreamReader( Request.MapPath( _fileImport ) ) )
					{
						YAF.Classes.Data.Import.DataImport.FileExtensionImport( PageBoardID, fileExtStream.BaseStream );
						fileExtStream.Close();
					}
				}
			}
			catch ( Exception x )
			{
				AddLoadMessage( x.Message );
				return false;
			}
			return true;
		}
		#endregion

		public string GetMembershipErrorMessage( MembershipCreateStatus status )
		{
			switch ( status )
			{
				case MembershipCreateStatus.DuplicateUserName:
					return "Username already exists. Please enter a different user name.";

				case MembershipCreateStatus.DuplicateEmail:
					return "A username for that e-mail address already exists. Please enter a different e-mail address.";

				case MembershipCreateStatus.InvalidPassword:
					return "The password provided is invalid. Please enter a valid password value.";

				case MembershipCreateStatus.InvalidEmail:
					return "The e-mail address provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidAnswer:
					return "The password retrieval answer provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidQuestion:
					return "The password retrieval question provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidUserName:
					return "The user name provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.ProviderError:
					return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				case MembershipCreateStatus.UserRejected:
					return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				default:
					return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
			}
		}

		#region property PageBoardID
		private int PageBoardID
		{
			get
			{
				try
				{
					return int.Parse( YAF.Classes.Config.BoardID );
				}
				catch
				{
					return 1;
				}
			}
		}
		#endregion

		#region method ExecuteScript
		private void ExecuteScript( string scriptFile, bool useTransactions )
		{
			string script = null;
			try
			{
				using ( System.IO.StreamReader file = new System.IO.StreamReader( Request.MapPath( scriptFile ) ) )
				{
					script = file.ReadToEnd() + "\r\n";

					file.Close();
				}
			}
			catch ( System.IO.FileNotFoundException )
			{
				return;
			}
			catch ( Exception x )
			{
				throw new Exception( "Failed to read " + scriptFile, x );
			}
			

            DB.system_initialize_executescripts(script,scriptFile, useTransactions);
	
		}
		#endregion

		#region method FixAccess
		private void FixAccess( bool bGrant )
		{
            DB.system_initialize_fixaccess(bGrant);		
		}
		#endregion
}
}
