using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read coil functions/requests.
    /// </summary>
    public class ReadCoilsFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadCoilsFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
		public ReadCoilsFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc/>
        public override byte[] PackRequest()
        {
            //TO DO: IMPLEMENT
            byte[] request = new byte[12];

            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.TransactionId)), 0, request, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.ProtocolId)), 0, request, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.Length)), 0, request, 4, 2);
            request[6] = CommandParameters.UnitId;
            request[7] = CommandParameters.FunctionCode;
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)((ModbusReadCommandParameters)CommandParameters).StartAddress)), 0, request, 8, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)(((ModbusReadCommandParameters)CommandParameters).Quantity))), 0, request, 10, 2);

            return request;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            //TO DO: IMPLEMENT
            Dictionary<Tuple<PointType, ushort>, ushort> dict = new Dictionary<Tuple<PointType, ushort>, ushort>();
            if ((response[7] & 0x80) == 0)
            {
                int byteCount = response[8];
                int quantity = ((ModbusReadCommandParameters)CommandParameters).Quantity;
                ushort startAddress = ((ModbusReadCommandParameters)CommandParameters).StartAddress;
                for (int i = 0; i < byteCount && quantity > 0; i++)
                {
                    var data = response[9 + i];
                    for (int j = 0; j < 8 && quantity > 0; j++, quantity--)
                    {
                        var value = (ushort)(data & 1);
                        dict.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, startAddress++), value);
                        data >>= 1;
                    }
                }
            }
            else
            {
                HandeException(response[8]);
            }
            return dict;
        }
    }
}