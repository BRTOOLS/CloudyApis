using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;

namespace CloudyApi
{
	// Token: 0x02000002 RID: 2
	public static class Api
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static bool CheckRobloxVersion()
		{
			Process[] processesByName = Process.GetProcessesByName("RobloxPlayerBeta");
			bool flag = processesByName.Length == 0;
			bool result;
			if (flag)
			{
				MessageBox.Show("Roblox is not running.", "CloudyApi");
				result = false;
			}
			else
			{
				string fileName = processesByName[0].MainModule.FileName;
				string name = new DirectoryInfo(Path.GetDirectoryName(fileName)).Name;
				bool flag2 = name != "version-2b67309334b54dab";
				if (flag2)
				{
					MessageBox.Show("CloudyApi is Outdated. Please Update https://cloudyweb.vercel.app", "CloudyApi");
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06000002 RID: 2
		[DllImport("bin\\Cloudy.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Initialize();

		// Token: 0x06000003 RID: 3
		[DllImport("bin\\Cloudy.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private static extern void Execute(byte[] scriptSource, string[] clientUsers, int numUsers);

		// Token: 0x06000004 RID: 4
		[DllImport("bin\\Cloudy.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr GetClients();

		// Token: 0x06000005 RID: 5 RVA: 0x000020D5 File Offset: 0x000002D5
		static Api()
		{
			Api.AutoSetup();
			Api.client.DefaultRequestHeaders.Add("Authorization", "v1USERFREE");
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002114 File Offset: 0x00000314
		public static void AutoInject(bool enable)
		{
			bool flag = Api.isRobloxOpen();
			if (flag)
			{
				bool flag2 = !Api.CheckRobloxVersion();
				if (!flag2)
				{
					Api.Initialize();
					Api._autoInject = enable;
					bool flag3 = !enable;
					if (!flag3)
					{
						Api.inject();
					}
				}
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x0000215C File Offset: 0x0000035C
		public static bool IsAutoInjectEnabled()
		{
			return Api._autoInject;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002164 File Offset: 0x00000364
		public static void inject()
		{
			Api.Initialize();
			Thread.Sleep(1000);
			string s = "\tgame:GetService(\"StarterGui\"):SetCore(\"SendNotification\", {\r\n\t\tTitle = \"[Cloudy API]\",\r\n\t\tText = \"Injected!\"\r\n\t})";
			string[] array = (from c in Api.GetClientsList()
			select c.name).ToArray<string>();
			Api.Execute(Encoding.UTF8.GetBytes(s), array, array.Length);
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000021D0 File Offset: 0x000003D0
		public static void execute(string scriptSource)
		{
			string[] array = (from c in Api.GetClientsList()
			select c.name).ToArray<string>();
			Api.Execute(Encoding.UTF8.GetBytes(scriptSource), array, array.Length);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002224 File Offset: 0x00000424
		[DebuggerStepThrough]
		public static Task<string> AskAi(string input)
		{
			Api.<AskAi>d__14 <AskAi>d__ = new Api.<AskAi>d__14();
			<AskAi>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<AskAi>d__.input = input;
			<AskAi>d__.<>1__state = -1;
			<AskAi>d__.<>t__builder.Start<Api.<AskAi>d__14>(ref <AskAi>d__);
			return <AskAi>d__.<>t__builder.Task;
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002268 File Offset: 0x00000468
		public static string Cleanres(string res)
		{
			return Regex.Replace(res, "^```lua\\s*(.*?)\\s*```$", "$1", RegexOptions.Singleline);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x0000228C File Offset: 0x0000048C
		public static List<Api.ClientInfo> GetClientsList()
		{
			List<Api.ClientInfo> list = new List<Api.ClientInfo>();
			IntPtr intPtr = Api.GetClients();
			for (;;)
			{
				Api.ClientInfo clientInfo = Marshal.PtrToStructure<Api.ClientInfo>(intPtr);
				bool flag = clientInfo.name != null;
				if (!flag)
				{
					break;
				}
				list.Add(clientInfo);
				intPtr += Marshal.SizeOf<Api.ClientInfo>();
			}
			return list;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000022E4 File Offset: 0x000004E4
		public static bool IsInjected()
		{
			bool result;
			try
			{
				result = (Api.GetClientsList().Count > 0);
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x0000231C File Offset: 0x0000051C
		public static void killRoblox()
		{
			foreach (Process process in Process.GetProcessesByName("RobloxPlayerBeta"))
			{
				try
				{
					process.Kill();
					process.WaitForExit();
				}
				catch (Exception ex)
				{
					Console.WriteLine("Failed to kill process: " + ex.Message);
				}
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002388 File Offset: 0x00000588
		public static string GetUsername()
		{
			string userName = Environment.UserName;
			string path = "C:\\\\Users\\\\" + userName + "\\\\AppData\\\\Local\\\\Roblox\\\\LocalStorage\\\\appStorage.json";
			bool flag = !File.Exists(path);
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				try
				{
					string text = File.ReadAllText(path);
					JObject jobject = JObject.Parse(text);
					bool flag2 = jobject.ContainsKey("Username");
					if (flag2)
					{
						JToken jtoken = jobject["Username"];
						return (jtoken != null) ? jtoken.ToString() : null;
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Error getting username " + ex.Message, "CloudyApi");
				}
				result = null;
			}
			return result;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002438 File Offset: 0x00000638
		public static bool isRobloxOpen()
		{
			return Process.GetProcessesByName("RobloxPlayerBeta").Any<Process>();
		}

		// Token: 0x06000011 RID: 17 RVA: 0x0000245C File Offset: 0x0000065C
		private static void AutoSetup()
		{
			string[] array = new string[]
			{
				"Cloudy.dll",
				"libcrypto-3-x64.dll",
				"libssl-3-x64.dll",
				"xxhash.dll",
				"zstd.dll"
			};
			string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
			bool flag = !Directory.Exists(text);
			if (flag)
			{
				Directory.CreateDirectory(text);
			}
			foreach (string text2 in array)
			{
				string text3 = Path.Combine(text, text2);
				bool flag2 = !File.Exists(text3);
				if (flag2)
				{
					try
					{
						string address = "https://github.com/volxphy1/cloudy-frontend/releases/download/DLLS/" + text2;
						using (WebClient webClient = new WebClient())
						{
							webClient.DownloadFile(address, text3);
						}
					}
					catch (Exception ex)
					{
						MessageBox.Show("Failed to Download " + text2 + ": " + ex.Message, "CloudyApi");
					}
				}
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000257C File Offset: 0x0000077C
		public static BitmapImage GetAvatar(string username)
		{
			string userIdFromUsername = Api.GetUserIdFromUsername(username);
			bool flag = string.IsNullOrEmpty(userIdFromUsername);
			BitmapImage result;
			if (flag)
			{
				result = null;
			}
			else
			{
				string address = "https://thumbnails.roblox.com/v1/users/avatar-headshot?userIds=" + userIdFromUsername + "&size=420x420&format=png";
				try
				{
					using (WebClient webClient = new WebClient())
					{
						string text = webClient.DownloadString(address);
						JObject jobject = JObject.Parse(text);
						bool flag2 = jobject["data"] != null && jobject["data"].HasValues;
						if (flag2)
						{
							string uriString = jobject["data"][0]["imageUrl"].ToString();
							return new BitmapImage(new Uri(uriString));
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Error getting avatar for " + username + ": " + ex.Message, "CloudyApi");
				}
				result = null;
			}
			return result;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002690 File Offset: 0x00000890
		private static string GetUserIdFromUsername(string username)
		{
			string address = "https://users.roblox.com/v1/usernames/users";
			string data = "{\"usernames\": [\"" + username + "\"]}";
			try
			{
				using (WebClient webClient = new WebClient())
				{
					webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
					string text = webClient.UploadString(address, "POST", data);
					JObject jobject = JObject.Parse(text);
					bool flag = jobject["data"] != null && jobject["data"].HasValues;
					if (flag)
					{
						return jobject["data"][0]["id"].ToString();
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error getting user ID for " + username + ": " + ex.Message, "CloudyApi");
			}
			return null;
		}

		// Token: 0x04000001 RID: 1
		private static System.Windows.Forms.Timer time12 = new System.Windows.Forms.Timer();

		// Token: 0x04000002 RID: 2
		private static bool isua = false;

		// Token: 0x04000003 RID: 3
		private static bool _autoInject;

		// Token: 0x04000004 RID: 4
		private static readonly HttpClient client = new HttpClient();

		// Token: 0x04000005 RID: 5
		private const string RequiredVersion = "version-2b67309334b54dab";

		// Token: 0x02000003 RID: 3
		public struct ClientInfo
		{
			// Token: 0x04000006 RID: 6
			public string version;

			// Token: 0x04000007 RID: 7
			public string name;

			// Token: 0x04000008 RID: 8
			public int id;
		}
	}
}
