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
using System.Linq;
using System.Collections.Generic;
using FTOptix.NativeUI;
using System.Threading;
using FTOptix.EventLogger;
using FTOptix.Report;
using FTOptix.DataLogger;
using System.ComponentModel;
using CoT;
#endregion


public class RT_CoT_SelectionInterfaceLogic : BaseNetLogic
{
	private IUAVariable _selectedOptionPointer;
	private IUAVariable _selectedID;

	public override void Start()
	{
		_selectedOptionPointer = LogicObject.GetVariable("selectedOption");
		_selectedID = LogicObject.GetVariable("selectedID");
		_selectedID.VariableChange += OnSelectedIdChanged;

		InitTyp dropdownInitializationMode = (InitTyp)(int)LogicObject.GetVariable("dropdownInitializationMode").Value;
		Initialize(dropdownInitializationMode);

	}

	private void Initialize(InitTyp initTyp)
	{
		if (InitTyp.ById == initTyp)
		{
			SelectOptionById(_selectedID.Value);
		}
		if (InitTyp.ByFirstElement == initTyp)
		{
			ResetSelectOptionToFirstElement();
		}
	}

	public override void Stop()
	{
		_selectedID.VariableChange -= OnSelectedIdChanged;
	}

	private void OnSelectedIdChanged(object sender, VariableChangeEventArgs e)
	{
		if (_selectedOptionPointer.Value==null|| e.NewValue != InformationModel.Get(_selectedOptionPointer.Value)?.GetVariable("id")?.Value)
		{
			SelectOptionById(e.NewValue);
		}
	}
	[ExportMethod]
	public void ResetSelectOptionToFirstElement()
	{
		try
		{
			List<CoT_SelectionOption> elements = Owner.Children.OfType<CoT_SelectionOption>().ToList();
			_selectedID.Value = elements.First().id;
			SelectOptionById(elements.First().id);
		}
		catch
		{
			Log.Warning(Owner.BrowseName, "First Element could not be selected");
		}
	}
	private void SelectOptionById(int id)
	{
		List<CoT_SelectionOption> elements = Owner.Children.OfType<CoT_SelectionOption>().ToList();
		foreach (var element in elements)
		{
			if (element.id == id)
			{
				_selectedOptionPointer.Value = element.NodeId;
				return;
			}
		}
		Log.Warning(Owner.BrowseName, "Selected id is invalid");
		try
		{
			_selectedID.Value = InformationModel.Get<CoT_SelectionOption>(_selectedOptionPointer.Value).id;
		}
		catch
		{
			Log.Warning(Owner.BrowseName, "Selected Pointer is Empty");
			ResetSelectOptionToFirstElement();
		}
	}


}
