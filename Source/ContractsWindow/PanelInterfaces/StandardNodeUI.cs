﻿#region license
/*The MIT License (MIT)
Contract Assembly - Monobehaviour To Check For Other Addons And Their Methods

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
using ContractsWindow.Unity.Interfaces;
using ProgressParser;
using UnityEngine;

namespace ContractsWindow.PanelInterfaces
{
	public class StandardNodeUI : IStandardNode
	{
		private progressStandard node;

		public StandardNodeUI(progressStandard n)
		{
			if (n == null)
				return;

			node = n;
		}

		public bool IsComplete
		{
			get
			{
				if (node == null)
					return false;

				return node.IsComplete;
			}
		}

		public string GetNote
		{
			get
			{
				if (node == null)
					return "";

				return string.Format(node.Note, node.NoteReference, node.KSPDateString);
			}
		}
		
		public string NodeText
		{
			get
			{
				if (node == null)
					return "";

				string body = node.Body == null ? "" : node.Body.theName;

				return string.Format(node.Descriptor, body);
			}
		}

		private string coloredText(string s, char c, string color)
		{
			if (string.IsNullOrEmpty(s))
				return "";

			return string.Format("<color={0}>{1}{2}</color>  ", color, c, s);
		}

		public string RewardText
		{
			get
			{
				if (node == null)
					return "";

				return string.Format("{0}{1}{2}", coloredText(node.FundsRewardString, '£', "#69D84FFF"), coloredText(node.SciRewardString, '©', "#02D8E9FF"), coloredText(node.RepRewardString, '¡', "#C9B003FF"));
			}
		}

		public void ProcessStyles(GameObject obj)
		{
			contractUtils.processComponents(obj);
		}
	}
}
