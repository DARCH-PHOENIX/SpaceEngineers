﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRage.Collections;
using VRage.Library.Sync;
using VRage.Serialization;

namespace VRage
{
    public class SyncType
    {
        List<SyncBase> m_properties;
        Action<SyncBase> m_registeredHandlers;

        public ListReader<SyncBase> Properties { get { return new ListReader<SyncBase>(m_properties); } }

        public event Action<SyncBase> PropertyChanged
        {
            add
            {
                m_registeredHandlers += value;
                foreach (var p in m_properties)
                {
                    p.ValueChanged += value;
                }
            }
            remove
            {
                foreach (var p in m_properties)
                {
                    p.ValueChanged -= value;
                }
                m_registeredHandlers -= value;
            }
        }

        public SyncType(List<SyncBase> properties)
        {
            m_properties = properties;
        }

#if !UNSHAPER_TMP
        public Sync<T> Add<T>(MySerializeInfo info = null)
        {
            var sync = new Sync<T>(m_properties.Count, info ?? MySerializeInfo.Default);
            sync.ValueChanged += m_registeredHandlers;
            m_properties.Add(sync);
            return sync;
        }
#endif

        public void Append(object obj)
        {
#if !UNSHAPER_TMP
            var num = m_properties.Count;
            SyncHelpers.Compose(obj, m_properties.Count, m_properties);
            for (int i = num; i < m_properties.Count; i++)
            {
                m_properties[i].ValueChanged += m_registeredHandlers;
            }
#endif
        }
    }
}
