﻿using D3DLab.ECS.Components;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;

namespace D3DLab.ECS {
    public interface IGeometryData : IDisposable {
        ImmutableArray<Vector3> Positions { get; }
        ImmutableArray<int> Indices { get; }
        ImmutableArray<Vector3> Normals { get; }
        ImmutableArray<Vector2> TexCoor { get; }
        bool IsModified { get; set; }
        bool IsDisposed { get; }
    }

    public interface IGeometryMemoryPool : ISynchronizationContext, IDisposable {
        TGeoData GetGeometry<TGeoData>(GraphicEntity entity) where TGeoData : IGeometryData;
        TGeoData GetGeometry<TGeoData>(GeometryPoolComponent com) where TGeoData : IGeometryData;
        GeometryPoolComponent AddGeometry<TGeoData>(TGeoData geo) where TGeoData : IGeometryData;
        void AddGeometry<TGeoData>(GraphicEntity entity, TGeoData data) where TGeoData : IGeometryData;

        GeometryPoolComponent UpdateGeometry<TGeoData>(GeometryPoolComponent old, TGeoData newGeo)
            where TGeoData : IGeometryData;
    }
}