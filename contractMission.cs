﻿#region license
/*The MIT License (MIT)
Contract Mission - Object to hold info about a mission list

Copyright (c) 2014 DMagic

KSP Plugin Framework by TriggerAu, 2014: http://forum.kerbalspaceprogram.com/threads/66503-KSP-Plugin-Framework

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;

namespace ContractsWindow
{
	/// <summary>
	/// A list of contracts; each with its own sort order and separate lists for active and hidden contracts
	/// A special master mission list is used to store all contracts and is used as the source for all other missions
	/// </summary>
	public class contractMission
	{
		private string name;
		private string activeString;
		private string hiddenString;
		private Dictionary<Guid, contractUIObject> missionList;
		private List<Guid> activeMissionList;
		private List<Guid> hiddenMissionList;
		private bool ascendingOrder = true;
		private bool showActiveMissions = true;
		private sortClass orderMode = sortClass.Difficulty;
		private bool masterMission;

		public string Name
		{
			get { return name; }
		}

		public int ActiveContracts
		{
			get { return missionList.Count; }
		}

		public List<Guid> ActiveMissionList
		{
			get { return activeMissionList; }
		}

		public List<Guid> HiddenMissionList
		{
			get { return hiddenMissionList; }
		}

		public bool AscendingOrder
		{
			get { return ascendingOrder; }
			internal set { ascendingOrder = value; }
		}

		public bool ShowActiveMissions
		{
			get { return showActiveMissions; }
			internal set { showActiveMissions = value; }
		}

		public sortClass OrderMode
		{
			get { return orderMode; }
			internal set { orderMode = value; }
		}

		public bool MasterMission
		{
			get { return masterMission; }
			internal set { masterMission = value; }
		}

		internal contractMission(string n, string active, string hidden, bool asc, bool showActive, int sMode, bool Master)
		{
			name = n;
			activeString = active;
			hiddenString = hidden;
			ascendingOrder = asc;
			showActiveMissions = showActive;
			masterMission = Master;
			orderMode = (sortClass)sMode;
			missionList = new Dictionary<Guid, contractUIObject>();
			activeMissionList = new List<Guid>();
			hiddenMissionList = new List<Guid>();
		}

		internal contractMission(string n)
		{
			name = n;
			missionList = new Dictionary<Guid, contractUIObject>();
			activeMissionList = new List<Guid>();
			hiddenMissionList = new List<Guid>();
		}

		internal contractUIObject getContract(Guid id)
		{
			if (missionList.ContainsKey(id))
				return missionList[id];
			else
				return null;
		}

		internal void buildMissionList()
		{
			resetMasterList();
			buildMissionList(activeString, true);
			buildMissionList(hiddenString, false);
		}

		private void buildMissionList(string s, bool Active)
		{
			if (string.IsNullOrEmpty(s))
				return;
			else
			{
				string[] sA = s.Split(',');
				for (int i = 0; i < sA.Length; i++)
				{
					contractContainer c = null;
					contractUIObject cUI = null;
					string[] sB = sA[i].Split('|');
					try
					{
						Guid g = new Guid(sB[0]);
						if (g == null)
							continue;

						c = contractScenario.Instance.getContract(g);
						if (c == null)
							continue;

						addContract(c, Active, true);
						cUI = getContract(g);
						if (cUI == null)
							continue;

						cUI.Order = stringIntParse(sB[1]);
						cUI.ShowParams = stringBoolParse(sB[2]);
					}
					catch (Exception e)
					{
						DMC_MBW.LogFormatted("Guid invalid: {0}", e);
						continue;
					}
				}
			}
		}

		internal List<Guid> loadPinnedContracts(List<Guid> gID)
		{
			List<contractUIObject> temp = new List<contractUIObject>();
			List<Guid> idTemp = new List<Guid>();
			foreach (Guid id in gID)
			{
				contractUIObject c = getContract(id);
				if (c != null)
				{
					if (c.Order != null)
						temp.Add(c);
				}
			}
			if (temp.Count > 0)
			{
				temp.Sort((a, b) =>
				{
					return Comparer<int?>.Default.Compare(a.Order, b.Order);
				});
				foreach (contractUIObject c in temp)
				{
					idTemp.Add(c.Container.Contract.ContractGuid);
				}
			}
			return idTemp;
		}

		private bool stringBoolParse(string source)
		{
			bool b;
			if (bool.TryParse(source, out b))
				return b;
			return true;
		}

		private int? stringIntParse(string s)
		{
			int i;
			if (int.TryParse(s, out i)) return i;
			return null;
		}

		internal void addContract(contractContainer c, bool active, bool warn)
		{
			if (!activeMissionList.Contains(c.Contract.ContractGuid) && !hiddenMissionList.Contains(c.Contract.ContractGuid))
			{
				if (addToMasterList(c))
				{
					if (active)
						activeMissionList.Add(c.Contract.ContractGuid);
					else
						hiddenMissionList.Add(c.Contract.ContractGuid);
				}
			}
			else if (warn)
				DMC_MBE.LogFormatted("Mission List Already Contains Contract: {0}", c.Title);
		}

		private bool addToMasterList(contractContainer c)
		{
			if (!missionList.ContainsKey(c.Contract.ContractGuid))
			{
				missionList.Add(c.Contract.ContractGuid, new contractUIObject(c));
				return true;
			}
			else
				DMC_MBE.LogFormatted("Master Mission List For: [{0}] Already Contains Contract: [{1}]", name, c.Title);

			return false;
		}

		internal void removeMission(contractContainer c)
		{
			foreach(Guid g in activeMissionList)
			{
				if (g == c.Contract.ContractGuid)
					activeMissionList.Remove(g);
			}

			foreach(Guid g in hiddenMissionList)
			{
				if (g == c.Contract.ContractGuid)
					hiddenMissionList.Remove(g);
			}

			removeFromMasterList(c);
		}

		private void removeFromMasterList(contractContainer c)
		{
			if (missionList.ContainsKey(c.Contract.ContractGuid))
				missionList.Remove(c.Contract.ContractGuid);
		}

		private void resetMasterList()
		{
			missionList.Clear();
		}

		internal string stringConcat(List<Guid> source)
		{
			if (source.Count == 0)
				return "";
			List<string> s = new List<string>();
			for (int j = 0; j < source.Count; j++)
			{
				contractUIObject c = getContract(source[j]);
				if (c == null)
					continue;
				string i;
				if (c.Order == null)
					i = "N";
				else
					i = c.Order.ToString();
				bool show = c.ShowParams;
				string id = string.Format("{0}|{1}|{2}", source[j], i, show);
				s.Add(id);
			}

			return string.Join(",", s.ToArray());
		}
	}
}
