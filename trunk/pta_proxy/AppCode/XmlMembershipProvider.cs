#region Using

using System;
using System.Xml;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Web.Security;
using System.Web.Hosting;
using System.Web.Management;
using System.Security.Permissions;
using System.Web;
using System.Text;
using System.Security.Cryptography;

#endregion

public class XmlMembershipProvider : MembershipProvider
{
  private Dictionary<string, MembershipUser> _Users;
  private string _XmlFileName;

  #region Properties

  // MembershipProvider Properties
  public override string ApplicationName
  {
    get { throw new NotSupportedException(); }
    set { throw new NotSupportedException(); }
  }

  public override bool EnablePasswordRetrieval
  {
    get { return false; }
  }

  public override bool EnablePasswordReset
  {
    get { return false; }
  }

  public override int MaxInvalidPasswordAttempts
  {
    get { return 5; }
  }

  public override int MinRequiredNonAlphanumericCharacters
  {
    get { return 0; }
  }

  public override int MinRequiredPasswordLength
  {
    get { return 8; }
  }

  public override int PasswordAttemptWindow
  {
    get { throw new NotSupportedException(); }
  }

  public override MembershipPasswordFormat PasswordFormat
  {
    get { return MembershipPasswordFormat.Clear; }
  }

  public override string PasswordStrengthRegularExpression
  {
    get { throw new NotSupportedException(); }
  }

  public override bool RequiresQuestionAndAnswer
  {
    get { return false; }
  }

  public override bool RequiresUniqueEmail
  {
    get { return false; }
  }

  #endregion

  #region Supported methods

  public override void Initialize(string name, NameValueCollection config)
  {
      log4net.ILog log = Idla.PtaProxy.PtaUtil.getLog4netLogger(this.GetType().FullName + ".Initialize(): ");  //(!!)
      try { //(!!)
          if (config == null)
              throw new ArgumentNullException("config");

          if (String.IsNullOrEmpty(name))
              name = "XmlMembershipProvider";

          if (string.IsNullOrEmpty(config["description"])) {
              config.Remove("description");
              config.Add("description", "XML membership provider");
          }

          base.Initialize(name, config);

          // Initialize _XmlFileName and make sure the path
          // is app-relative
          string path = config["xmlFileName"];

          if (String.IsNullOrEmpty(path))
              path = "~/App_Data/Users.xml";

          if (!VirtualPathUtility.IsAppRelative(path))
              throw new ArgumentException
                  ("xmlFileName must be app-relative");

          string fullyQualifiedPath = VirtualPathUtility.Combine
              (VirtualPathUtility.AppendTrailingSlash
              (HttpRuntime.AppDomainAppVirtualPath), path);

          _XmlFileName = HostingEnvironment.MapPath(fullyQualifiedPath);
          config.Remove("xmlFileName");

          // Make sure we have permission to read the XML data source and
          // throw an exception if we don't
          FileIOPermission permission = new FileIOPermission(FileIOPermissionAccess.Write, _XmlFileName);
          permission.Demand();

          // Throw an exception if unrecognized attributes remain
          if (config.Count > 0) {
              string attr = config.GetKey(0);
              if (!String.IsNullOrEmpty(attr))
                  throw new ProviderException("Unrecognized attribute: " + attr);
          }
      } catch (Exception exc) {     //(!!)
          log.Error(exc);   //(!!)
          throw;        //(!!)
      }
  }

  /// <summary>
  /// Returns true if the username and password match an exsisting user.
  /// </summary>
  public override bool ValidateUser(string username, string password)
  {
      log4net.ILog log = Idla.PtaProxy.PtaUtil.getLog4netLogger(this.GetType().FullName + ".ValidateUser(): ");  //(!!)
      if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password)) {
          log.Debug("if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))");
          return false;
      }
    try
    {
      ReadMembershipDataStore();

      // Validate the user name and password
      MembershipUser user;
      if (_Users.TryGetValue(username, out user))
      {
          log.Debug("if (_Users.TryGetValue(username, out user))");
        if (user.Comment == Encrypt(password)) // Case-sensitive
        {
          user.LastLoginDate = DateTime.Now;
          UpdateUser(user);
          return true;
        }
      }

      return false;
    }
    catch (Exception ex)
    {
        log.Error(ex);  //(!!)
      return false;
    }
  }

  /// <summary>
  /// Retrieves a user based on his/hers username.
  /// the userIsOnline parameter is ignored.
  /// </summary>
  public override MembershipUser GetUser(string username, bool userIsOnline)
  {
    if (String.IsNullOrEmpty(username))
      return null;

    ReadMembershipDataStore();

    // Retrieve the user from the data source
    MembershipUser user;
    if (_Users.TryGetValue(username, out user))
      return user;

    return null;
  }

  /// <summary>
  /// Retrieves a collection of all the users.
  /// This implementation ignores pageIndex and pageSize,
  /// and it doesn't sort the MembershipUser objects returned.
  /// </summary>
  public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
  {
    ReadMembershipDataStore();
    MembershipUserCollection users = new MembershipUserCollection();

    foreach (KeyValuePair<string, MembershipUser> pair in _Users)
    {
      users.Add(pair.Value);
    }

    totalRecords = users.Count;
    return users;
  }

  /// <summary>
  /// Changes a users password.
  /// </summary>
  public override bool ChangePassword(string username, string oldPassword, string newPassword)
  {
    XmlDocument doc = new XmlDocument();
    doc.Load(_XmlFileName);
    XmlNodeList nodes = doc.GetElementsByTagName("User");
    foreach (XmlNode node in nodes)
    {
      if (node["UserName"].InnerText.Equals(username, StringComparison.OrdinalIgnoreCase)
//(!!)        || 
          && node["Password"].InnerText.Equals(Encrypt(oldPassword), StringComparison.OrdinalIgnoreCase))
      {
        node["Password"].InnerText = Encrypt(newPassword);
        doc.Save(_XmlFileName);
        return true;
      }
    }

    return false;
  }

  /// <summary>
  /// Creates a new user store he/she in the XML file
  /// </summary>
  public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
  {
    XmlDocument doc = new XmlDocument();
    doc.Load(_XmlFileName);

    XmlNode xmlUserRoot = doc.CreateElement("User");
    XmlNode xmlUserName = doc.CreateElement("UserName");
    XmlNode xmlPassword = doc.CreateElement("Password");
    XmlNode xmlEmail = doc.CreateElement("Email");
    XmlNode xmlLastLoginTime = doc.CreateElement("LastLoginTime");

    xmlUserName.InnerText = username;
    xmlPassword.InnerText = Encrypt(password);
    xmlEmail.InnerText = email;
    xmlLastLoginTime.InnerText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

    xmlUserRoot.AppendChild(xmlUserName);
    xmlUserRoot.AppendChild(xmlPassword);
    xmlUserRoot.AppendChild(xmlEmail);
    xmlUserRoot.AppendChild(xmlLastLoginTime);

    doc.SelectSingleNode("Users").AppendChild(xmlUserRoot);
    doc.Save(_XmlFileName);

    status = MembershipCreateStatus.Success;
    MembershipUser user = new MembershipUser(Name, username, username, email, passwordQuestion, Encrypt(password), isApproved, false, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.MaxValue);
    _Users.Add(username, user);
    return user;
  }

  /// <summary>
  /// Deletes the user from the XML file and 
  /// removes him/her from the internal cache.
  /// </summary>
  public override bool DeleteUser(string username, bool deleteAllRelatedData)
  {
    XmlDocument doc = new XmlDocument();
    doc.Load(_XmlFileName);

    foreach (XmlNode node in doc.GetElementsByTagName("User"))
    {
      if (node.ChildNodes[0].InnerText.Equals(username, StringComparison.OrdinalIgnoreCase))
      {
        doc.SelectSingleNode("Users").RemoveChild(node);
        doc.Save(_XmlFileName);
        _Users.Remove(username);
        return true;
      }
    }

    return false;
  }

  /// <summary>
  /// Get a user based on the username parameter.
  /// the userIsOnline parameter is ignored.
  /// </summary>
  public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
  {
    XmlDocument doc = new XmlDocument();
    doc.Load(_XmlFileName);

    foreach (XmlNode node in doc.SelectNodes("//User"))
    {
      if (node.ChildNodes[0].InnerText.Equals(providerUserKey.ToString(), StringComparison.OrdinalIgnoreCase))
      {
        string userName = node.ChildNodes[0].InnerText;
        string password = node.ChildNodes[1].InnerText;
        string email = node.ChildNodes[2].InnerText;
        DateTime lastLoginTime = DateTime.Parse(node.ChildNodes[3].InnerText);
        return new MembershipUser(Name, providerUserKey.ToString(), providerUserKey, email, string.Empty, password, true, false, DateTime.Now, lastLoginTime, DateTime.Now, DateTime.Now, DateTime.MaxValue);
      }
    }

    return default(MembershipUser);
  }

  /// <summary>
  /// Retrieves a username based on a matching email.
  /// </summary>
  public override string GetUserNameByEmail(string email)
  {
    XmlDocument doc = new XmlDocument();
    doc.Load(_XmlFileName);

    foreach (XmlNode node in doc.GetElementsByTagName("User"))
    {
      if (node.ChildNodes[2].InnerText.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase))
      {
        return node.ChildNodes[0].InnerText;
      }
    }

    return null;
  }

  /// <summary>
  /// Updates a user. The username will not be changed.
  /// </summary>
  public override void UpdateUser(MembershipUser user)
  {
    XmlDocument doc = new XmlDocument();
    doc.Load(_XmlFileName);

    foreach (XmlNode node in doc.GetElementsByTagName("User"))
    {
      if (node.ChildNodes[0].InnerText.Equals(user.UserName, StringComparison.OrdinalIgnoreCase))
      {
        if (user.Comment.Length > 30)
        {
            //(!!) this encrypted password second time 
          //(!!) node.ChildNodes[1].InnerText = Encrypt(user.Comment);
            node.ChildNodes[1].InnerText = user.Comment;
        }
        node.ChildNodes[2].InnerText = user.Email;
        node.ChildNodes[3].InnerText = user.LastLoginDate.ToString("yyyy-MM-dd HH:mm:ss");
        doc.Save(_XmlFileName);
        _Users[user.UserName] = user;
      }
    }
  }

  #endregion

  #region Helper methods

  /// <summary>
  /// Builds the internal cache of users.
  /// </summary>
  private void ReadMembershipDataStore()
  {
    lock (this)
    {
      if (_Users == null)
      {
        _Users = new Dictionary<string, MembershipUser>(16, StringComparer.InvariantCultureIgnoreCase);
        XmlDocument doc = new XmlDocument();
        doc.Load(_XmlFileName);
        XmlNodeList nodes = doc.GetElementsByTagName("User");

        foreach (XmlNode node in nodes)
        {
          MembershipUser user = new MembershipUser(
              Name,                       // Provider name
              node["UserName"].InnerText, // Username
              node["UserName"].InnerText, // providerUserKey
              node["Email"].InnerText,    // Email
              String.Empty,               // passwordQuestion
              node["Password"].InnerText, // Comment
              true,                       // isApproved
              false,                      // isLockedOut
              DateTime.Now,               // creationDate
              DateTime.Parse(node["LastLoginTime"].InnerText), // lastLoginDate
              DateTime.Now,               // lastActivityDate
              DateTime.Now, // lastPasswordChangedDate
              new DateTime(1980, 1, 1)    // lastLockoutDate
          );

          _Users.Add(user.UserName, user);
        }
      }
    }
  }

  /// <summary>
  /// Encrypts a string using the SHA256 algorithm.
  /// </summary>
  private static string Encrypt(string plainMessage)
  {
    byte[] data = Encoding.UTF8.GetBytes(plainMessage);
    using (HashAlgorithm sha = new SHA256Managed())
    {
      byte[] encryptedBytes = sha.TransformFinalBlock(data, 0, data.Length);
      return Convert.ToBase64String(sha.Hash);
    }
  }

  #endregion

  #region Unsupported methods

  public override string ResetPassword(string username, string answer)
  {
    throw new NotSupportedException();
  }

  public override bool UnlockUser(string userName)
  {
    throw new NotSupportedException();
  }

  public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
  {
    throw new NotSupportedException();
  }

  public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
  {
    throw new NotSupportedException();
  }

  public override int GetNumberOfUsersOnline()
  {
    throw new NotSupportedException();
  }

  public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
  {
    throw new NotSupportedException();
  }

  public override string GetPassword(string username, string answer)
  {
    throw new NotSupportedException();
  }

  #endregion

}