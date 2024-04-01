/*
****************************************************************************
*  Copyright (c) 2024,  Skyline Communications NV  All Rights Reserved.    *
****************************************************************************

By using this script, you expressly agree with the usage terms and
conditions set out below.
This script and all related materials are protected by copyrights and
other intellectual property rights that exclusively belong
to Skyline Communications.

A user license granted for this script is strictly for personal use only.
This script may not be used in any way by anyone without the prior
written consent of Skyline Communications. Any sublicensing of this
script is forbidden.

Any modifications to this script by the user are only allowed for
personal use and within the intended purpose of the script,
and will remain the sole responsibility of the user.
Skyline Communications will not be responsible for any damages or
malfunctions whatsoever of the script resulting from a modification
or adaptation by the user.

The content of this script is confidential information.
The user hereby agrees to keep this confidential information strictly
secret and confidential and not to disclose or reveal it, in whole
or in part, directly or indirectly to any person, entity, organization
or administration without the prior written consent of
Skyline Communications.

Any inquiries can be addressed to:

	Skyline Communications NV
	Ambachtenstraat 33
	B-8870 Izegem
	Belgium
	Tel.	: +32 51 31 35 69
	Fax.	: +32 51 31 01 29
	E-mail	: info@skyline.be
	Web		: www.skyline.be
	Contact	: Ben Vandenberghe

****************************************************************************
Revision History:

DATE		VERSION		AUTHOR			COMMENTS

19/03/2024	1.0.0.1		GBO, Skyline	Initial version
****************************************************************************
*/

namespace GENERIC_VIRTUAL_CONNECTOR_HISTORY_SET_1
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Text;
    using System.Text.RegularExpressions;
    using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallBulk;
    using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
    using Skyline.DataMiner.Core.InterAppCalls.Common.Shared;

    /// <summary>
    /// Represents a DataMiner Automation script.
    /// </summary>
	public class Script
	{
		/// <summary>
		/// The script entry point.
		/// </summary>
		/// <param name="engine">Link with SLAutomation process.</param>
		public void Run(IEngine engine)
		{
			try
			{
				var paramValues = new ParamValues(engine);

                var message = new GenericHistorySetMessage
				{
					PrimaryKey = paramValues.PrimaryKey,
					NumericRangeMinimum = paramValues.NumericRangeMinimum,
					NumericRangeMaximum = paramValues.NumericRangeMaximum,
					PercentageRangeMaximum = paramValues.PercentageRangeMaximum,
					HistoryTimespan = paramValues.HistoryTimespan,
					HistoryInterval = paramValues.HistoryInterval,
				};

				IInterAppCall command = InterAppCallFactory.CreateNew();
				command.Messages.Add(message);
				command.Source = new Source("GENERIC_VIRTUAL_CONNECTOR_HISTORY_SET");
				command.ReturnAddress = new ReturnAddress(paramValues.DmaId, paramValues.ElementId, 9000001);
				command.Send(Engine.SLNetRaw, paramValues.DmaId, paramValues.ElementId, 9000000, new List<Type> { typeof(GenericHistorySetMessage)});
            }
            catch (Exception e)
            {

                engine.Log(e.ToString());
            }
        }
    }

	public class ParamValues
	{
		public ParamValues(IEngine engine)
		{
			if(!Int32.TryParse(engine.GetScriptParam(2).Value, out int dmaId))
			{
				throw new ArgumentException("DMA ID not in right format");
			}

			if (!Int32.TryParse(engine.GetScriptParam(3).Value, out int elemId))
            {
                throw new ArgumentException("Element ID not in right format");
            }

			if (!Int32.TryParse(engine.GetScriptParam(7).Value, out int historyTimespan))
            {
                throw new ArgumentException("History Timespan not in right format");
            }

			if (!Int32.TryParse(engine.GetScriptParam(8).Value, out int historyInterval))
            {
                throw new ArgumentException("History Interval not in right format");
            }

			var rangeRegex = new Regex(@"(?<MinValue>\d+)\s?.\s?(?<MaxValue>\d+)");

			DmaId = dmaId;
			ElementId = elemId;
			PrimaryKey = engine.GetScriptParam(4).Value;

			var numericRangeMatch = rangeRegex.Match(engine.GetScriptParam(5).Value);
			if(!numericRangeMatch.Success)
			{
                throw new ArgumentException("Numeric Range not in right format");
            }

			NumericRangeMinimum = Convert.ToInt32(numericRangeMatch.Groups["MinValue"].Value);
			NumericRangeMaximum = Convert.ToInt32(numericRangeMatch.Groups["MaxValue"].Value);

			PercentageRangeMaximum = Convert.ToDouble(engine.GetScriptParam(6).Value, CultureInfo.InvariantCulture);

			if(PercentageRangeMaximum > 100)
			{
				PercentageRangeMaximum = 100;
			}

			HistoryTimespan = historyTimespan;
			HistoryInterval = historyInterval;
        }

		public int DmaId { get; set; }

		public int ElementId { get; set; }

		public string PrimaryKey { get; set; }

		public int NumericRangeMinimum { get; set; }

		public int NumericRangeMaximum { get; set; }

		public double PercentageRangeMaximum { get; set; }

		public int HistoryTimespan { get; set; }

		public int HistoryInterval { get; set; }
    }

    public class GenericHistorySetMessage : Message
    {
        public string PrimaryKey { get; set; }

        public int NumericRangeMinimum { get; set; }

        public int NumericRangeMaximum { get; set; }

        public double PercentageRangeMaximum { get; set; }

        public int HistoryTimespan { get; set; }

        public int HistoryInterval { get; set; }
    }
}