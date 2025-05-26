#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.SQLiteStore;
using FTOptix.WebUI;
using FTOptix.Recipe;
using FTOptix.Store;
using FTOptix.RAEtherNetIP;
using FTOptix.System;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.Alarm;
using FTOptix.CommunicationDriver;
using FTOptix.Core;
using System.Collections.Generic;
using System.Linq;
using FTOptix.NativeUI;
using FTOptix.EventLogger;
using FTOptix.Report;
using FTOptix.DataLogger;
#endregion

public class RT_CoT_DropdownContentLogic : BaseNetLogic
{
	private IUAVariable _headerActive;
	private IUANode _selectionInterface;
	private IUANode _content;
	private IUAVariable _contentHeight;
	private IUAVariable _maxVisibleOptions;

	public override void Start()
	{
		_headerActive = LogicObject.GetVariable("headerActive");
		_selectionInterface = InformationModel.Get(LogicObject.GetVariable("selectionInterface").Value);
		_content = InformationModel.Get(LogicObject.GetVariable("content").Value);
		_contentHeight = LogicObject.GetVariable("contentHeight");
		_maxVisibleOptions = LogicObject.GetVariable("maxVisibleOptions");
		Initialize();
	}

	public override void Stop()
	{
		_headerActive.Value = false;
	}

	/// <summary>
	/// Initialize the content of the dropdown
	/// </summary>
	public void Initialize()
	{
		List<CoT_SelectionOption> options = _selectionInterface.Children.OfType<CoT_SelectionOption>().ToList();

		if (options.Count > 0)
		{
			_contentHeight.Value = 0;
			int count = 0;
			foreach (var option in options)
			{
				if (!option.visible)
				{
					continue;
				}

				CoT_CMP_DropdownElement optionUI = InformationModel.Make<CoT_CMP_DropdownElement>($"Option{count}");
				optionUI.GetVariable("selectionOption").Value = option.NodeId;
				optionUI.GetVariable("selectionInterface").Value = _selectionInterface.NodeId;

				_content.Add(optionUI);
				if (count < _maxVisibleOptions.Value)
				{
					_contentHeight.Value += optionUI.Height;
				}
				count++;
			}
		}
		else
		{
			Log.Warning(Owner.BrowseName, "No options in dropdown");
		}
	}
}
