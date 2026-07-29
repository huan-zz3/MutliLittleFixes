using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using MissionSharedLibrary.Utilities;
using TaleWorlds.Core;

namespace MissionSharedLibrary.Config
{
	// Token: 0x0200003D RID: 61
	public abstract class MissionConfigBase<T> where T : MissionConfigBase<T>
	{
		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000211 RID: 529 RVA: 0x00007BFD File Offset: 0x00005DFD
		// (set) Token: 0x06000212 RID: 530 RVA: 0x00007C04 File Offset: 0x00005E04
		public static T Instance { get; set; }

		// Token: 0x06000213 RID: 531 RVA: 0x00007C0C File Offset: 0x00005E0C
		public static T Get()
		{
			if (MissionConfigBase<T>.Instance == null)
			{
				MissionConfigBase<T>.Instance = Activator.CreateInstance<T>();
				MissionConfigBase<T>.Instance.SyncWithSave();
			}
			return MissionConfigBase<T>.Instance;
		}

		// Token: 0x06000214 RID: 532 RVA: 0x00007C38 File Offset: 0x00005E38
		public static void Clear()
		{
			MissionConfigBase<T>.Instance = default(T);
		}

		// Token: 0x06000215 RID: 533
		protected abstract void CopyFrom(T other);

		// Token: 0x06000216 RID: 534
		protected abstract void UpgradeToCurrentVersion();

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000217 RID: 535 RVA: 0x00007C53 File Offset: 0x00005E53
		protected virtual XmlSerializer Serializer { get; } = new XmlSerializer(typeof(T));

		// Token: 0x06000218 RID: 536 RVA: 0x00007C5C File Offset: 0x00005E5C
		public virtual bool Serialize()
		{
			try
			{
				this.EnsureParentDirectory();
				XmlSerializer serializer = this.Serializer;
				using (TextWriter textWriter = new StreamWriter(this.SaveName, false, Encoding.UTF8))
				{
					serializer.Serialize(textWriter, this);
					return true;
				}
			}
			catch (Exception ex)
			{
				Utility.DisplayMessage(ex.ToString());
				Console.WriteLine(ex);
			}
			return false;
		}

		// Token: 0x06000219 RID: 537 RVA: 0x00007CD0 File Offset: 0x00005ED0
		public virtual bool Deserialize()
		{
			try
			{
				this.EnsureParentDirectory();
				XmlSerializer serializer = this.Serializer;
				using (TextReader textReader = new StreamReader(this.SaveName))
				{
					T t = (T)((object)serializer.Deserialize(textReader));
					this.CopyFrom(t);
				}
				this.UpgradeToCurrentVersion();
				return true;
			}
			catch (Exception ex)
			{
				Utility.DisplayMessage(ex.ToString());
				Console.WriteLine(ex);
			}
			return false;
		}

		// Token: 0x0600021A RID: 538 RVA: 0x00007D54 File Offset: 0x00005F54
		protected void SyncWithSave()
		{
			try
			{
				if (File.Exists(this.SaveName) && this.Deserialize())
				{
					this.RemoveOldConfig();
				}
				else
				{
					this.MoveOldConfig();
					if (!File.Exists(this.SaveName) || !this.Deserialize())
					{
						this.ResetToDefault();
						this.Serialize();
					}
				}
			}
			catch (Exception ex)
			{
				Utility.DisplayMessage(ex.ToString());
				Console.WriteLine(ex);
			}
		}

		// Token: 0x0600021B RID: 539 RVA: 0x00007DCC File Offset: 0x00005FCC
		public void ResetToDefault()
		{
			this.CopyFrom(Activator.CreateInstance<T>());
		}

		// Token: 0x0600021C RID: 540 RVA: 0x00007DDC File Offset: 0x00005FDC
		protected void RemoveOldConfig()
		{
			try
			{
				if (this.OldNames != null)
				{
					foreach (string text in this.OldNames)
					{
						if (File.Exists(text))
						{
							File.Delete(text);
						}
						if (this.OldSavePath != null && Directory.Exists(this.OldSavePath) && Directory.GetFileSystemEntries(this.OldSavePath).Length == 0)
						{
							Directory.Delete(this.OldSavePath);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Utility.DisplayMessage(ex.ToString());
				Console.WriteLine(ex);
			}
		}

		// Token: 0x0600021D RID: 541 RVA: 0x00007E6C File Offset: 0x0000606C
		private void MoveOldConfig()
		{
			try
			{
				string[] oldNames = this.OldNames;
				string text = ((oldNames != null) ? oldNames.FirstOrDefault<string>(new Func<string, bool>(File.Exists)) : null);
				if (text != null && !Extensions.IsEmpty<char>(text))
				{
					this.EnsureParentDirectory();
					File.Move(text, this.SaveName);
				}
				this.RemoveOldConfig();
			}
			catch (Exception ex)
			{
				Utility.DisplayMessage(ex.ToString());
				Console.WriteLine(ex);
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x0600021E RID: 542
		protected abstract string SaveName { get; }

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600021F RID: 543 RVA: 0x00007EE0 File Offset: 0x000060E0
		protected virtual string OldSavePath
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000220 RID: 544 RVA: 0x00007EE3 File Offset: 0x000060E3
		protected virtual string[] OldNames
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06000221 RID: 545 RVA: 0x00007EE8 File Offset: 0x000060E8
		protected void EnsureParentDirectory()
		{
			string directoryName = Path.GetDirectoryName(this.SaveName);
			if (directoryName != null)
			{
				Directory.CreateDirectory(directoryName);
			}
		}
	}
}
