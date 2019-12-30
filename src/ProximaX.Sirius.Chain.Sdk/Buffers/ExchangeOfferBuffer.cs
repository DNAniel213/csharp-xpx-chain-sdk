// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace ProximaX.Sirius.Chain.Sdk.Buffers
{

using global::System;
using global::FlatBuffers;

public struct ExchangeOfferBuffer : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static ExchangeOfferBuffer GetRootAsExchangeOfferBuffer(ByteBuffer _bb) { return GetRootAsExchangeOfferBuffer(_bb, new ExchangeOfferBuffer()); }
  public static ExchangeOfferBuffer GetRootAsExchangeOfferBuffer(ByteBuffer _bb, ExchangeOfferBuffer obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public ExchangeOfferBuffer __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public uint MosaicId(int j) { int o = __p.__offset(4); return o != 0 ? __p.bb.GetUint(__p.__vector(o) + j * 4) : (uint)0; }
  public int MosaicIdLength { get { int o = __p.__offset(4); return o != 0 ? __p.__vector_len(o) : 0; } }
#if ENABLE_SPAN_T
  public Span<byte> GetMosaicIdBytes() { return __p.__vector_as_span(4); }
#else
  public ArraySegment<byte>? GetMosaicIdBytes() { return __p.__vector_as_arraysegment(4); }
#endif
  public uint[] GetMosaicIdArray() { return __p.__vector_as_array<uint>(4); }
  public uint MosaicAmount(int j) { int o = __p.__offset(6); return o != 0 ? __p.bb.GetUint(__p.__vector(o) + j * 4) : (uint)0; }
  public int MosaicAmountLength { get { int o = __p.__offset(6); return o != 0 ? __p.__vector_len(o) : 0; } }
#if ENABLE_SPAN_T
  public Span<byte> GetMosaicAmountBytes() { return __p.__vector_as_span(6); }
#else
  public ArraySegment<byte>? GetMosaicAmountBytes() { return __p.__vector_as_arraysegment(6); }
#endif
  public uint[] GetMosaicAmountArray() { return __p.__vector_as_array<uint>(6); }
  public uint Cost(int j) { int o = __p.__offset(8); return o != 0 ? __p.bb.GetUint(__p.__vector(o) + j * 4) : (uint)0; }
  public int CostLength { get { int o = __p.__offset(8); return o != 0 ? __p.__vector_len(o) : 0; } }
#if ENABLE_SPAN_T
  public Span<byte> GetCostBytes() { return __p.__vector_as_span(8); }
#else
  public ArraySegment<byte>? GetCostBytes() { return __p.__vector_as_arraysegment(8); }
#endif
  public uint[] GetCostArray() { return __p.__vector_as_array<uint>(8); }
  public byte Type { get { int o = __p.__offset(10); return o != 0 ? __p.bb.Get(o + __p.bb_pos) : (byte)0; } }
  public byte Owner(int j) { int o = __p.__offset(12); return o != 0 ? __p.bb.Get(__p.__vector(o) + j * 1) : (byte)0; }
  public int OwnerLength { get { int o = __p.__offset(12); return o != 0 ? __p.__vector_len(o) : 0; } }
#if ENABLE_SPAN_T
  public Span<byte> GetOwnerBytes() { return __p.__vector_as_span(12); }
#else
  public ArraySegment<byte>? GetOwnerBytes() { return __p.__vector_as_arraysegment(12); }
#endif
  public byte[] GetOwnerArray() { return __p.__vector_as_array<byte>(12); }

  public static Offset<ExchangeOfferBuffer> CreateExchangeOfferBuffer(FlatBufferBuilder builder,
      VectorOffset mosaicIdOffset = default(VectorOffset),
      VectorOffset mosaicAmountOffset = default(VectorOffset),
      VectorOffset costOffset = default(VectorOffset),
      byte type = 0,
      VectorOffset ownerOffset = default(VectorOffset)) {
    builder.StartObject(5);
    ExchangeOfferBuffer.AddOwner(builder, ownerOffset);
    ExchangeOfferBuffer.AddCost(builder, costOffset);
    ExchangeOfferBuffer.AddMosaicAmount(builder, mosaicAmountOffset);
    ExchangeOfferBuffer.AddMosaicId(builder, mosaicIdOffset);
    ExchangeOfferBuffer.AddType(builder, type);
    return ExchangeOfferBuffer.EndExchangeOfferBuffer(builder);
  }

  public static void StartExchangeOfferBuffer(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddMosaicId(FlatBufferBuilder builder, VectorOffset mosaicIdOffset) { builder.AddOffset(0, mosaicIdOffset.Value, 0); }
  public static VectorOffset CreateMosaicIdVector(FlatBufferBuilder builder, uint[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddUint(data[i]); return builder.EndVector(); }
  public static VectorOffset CreateMosaicIdVectorBlock(FlatBufferBuilder builder, uint[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
  public static void StartMosaicIdVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddMosaicAmount(FlatBufferBuilder builder, VectorOffset mosaicAmountOffset) { builder.AddOffset(1, mosaicAmountOffset.Value, 0); }
  public static VectorOffset CreateMosaicAmountVector(FlatBufferBuilder builder, uint[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddUint(data[i]); return builder.EndVector(); }
  public static VectorOffset CreateMosaicAmountVectorBlock(FlatBufferBuilder builder, uint[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
  public static void StartMosaicAmountVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddCost(FlatBufferBuilder builder, VectorOffset costOffset) { builder.AddOffset(2, costOffset.Value, 0); }
  public static VectorOffset CreateCostVector(FlatBufferBuilder builder, uint[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddUint(data[i]); return builder.EndVector(); }
  public static VectorOffset CreateCostVectorBlock(FlatBufferBuilder builder, uint[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
  public static void StartCostVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddType(FlatBufferBuilder builder, byte type) { builder.AddByte(3, type, 0); }
  public static void AddOwner(FlatBufferBuilder builder, VectorOffset ownerOffset) { builder.AddOffset(4, ownerOffset.Value, 0); }
  public static VectorOffset CreateOwnerVector(FlatBufferBuilder builder, byte[] data) { builder.StartVector(1, data.Length, 1); for (int i = data.Length - 1; i >= 0; i--) builder.AddByte(data[i]); return builder.EndVector(); }
  public static VectorOffset CreateOwnerVectorBlock(FlatBufferBuilder builder, byte[] data) { builder.StartVector(1, data.Length, 1); builder.Add(data); return builder.EndVector(); }
  public static void StartOwnerVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(1, numElems, 1); }
  public static Offset<ExchangeOfferBuffer> EndExchangeOfferBuffer(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ExchangeOfferBuffer>(o);
  }
};


}
