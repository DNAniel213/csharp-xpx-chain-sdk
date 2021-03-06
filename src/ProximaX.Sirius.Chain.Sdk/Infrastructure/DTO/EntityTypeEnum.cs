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
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ProximaX.Sirius.Chain.Sdk.Infrastructure.DTO {

  /// <summary>
  /// The entity type: * 0x4158 (16728 decimal) - Blockchain Upgrade Transaction. * 0x4159 (16729 decimal) - Network Config Transaction. * 0x413D (16701 decimal) - Address Metadata Transaction. * 0x423D (16957 decimal) - Mosaic Metadata Transaction. * 0x433D (17213 decimal) - Namespace Metadata Transaction. * 0x414D (16717 decimal) - Mosaic Definition Transaction. * 0x424D (16973 decimal) - Mosaic Supply Change Transaction. * 0x414E (16718 decimal) - Register Namespace Transaction. * 0x424E (16974 decimal) - Address Alias Transaction. * 0x434E (17230 decimal) - Mosaic Alias Transaction. * 0x4154 (16724 decimal) - Transfer Transaction. * 0x4155 (16725 decimal) - Modify Multisig Account Transaction. * 0x4141 (16705 decimal) - Aggregate Complete Transaction. * 0x4241 (16961 decimal) - Aggregate Bonded Transaction. * 0x4148 (16712 decimal) - Hash Lock Transaction. * 0x4150 (16720 decimal) - Account Properties Address Transaction. * 0x4250 (16976 decimal) - Account Properties Mosaic Transaction. * 0x4350 (17232 decimal) - Account Properties Entity Type Transaction. * 0x4152 (16722 decimal) - Secret Lock Transaction. * 0x4252 (16978 decimal) - Secret Proof Transaction. * 0x414C (16716 decimal) - Account Link Transaction. * 0x8043 (32835 decimal) - Nemesis block. * 0x8143 (33091 decimal) - Regular block. 
  /// </summary>
  [DataContract]
  public enum EntityTypeEnum :int{



}
}
