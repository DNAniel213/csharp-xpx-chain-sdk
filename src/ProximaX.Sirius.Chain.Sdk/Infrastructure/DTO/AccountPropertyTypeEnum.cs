// Copyright 2019 ProximaX
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System.Runtime.Serialization;

namespace ProximaX.Sirius.Chain.Sdk.Infrastructure.DTO
{

    /// <summary>
    /// The account properties type: * 0x01 (1 decimal) - The property type only allows receiving transactions from an address. * 0x02 (2 decimal) - The property type only allows receiving transactions containing a mosaic id. * 0x04 (4 decimal) - The property type only allows sending transactions with a given transaction type. * 0x05 (5 decimal) - Property type sentinel. * 0x81 (129 decimal) - The property type blocks receiving transactions from an address. * 0x82 (130 decimal) - The property type blocks receiving transactions containing a mosaic id. * 0x84 (132 decimal) -  The property type blocks sending transactions with a given transaction type. 
    /// </summary>
    [DataContract]
    public enum AccountPropertyTypeEnum : int
    {



    }
}
