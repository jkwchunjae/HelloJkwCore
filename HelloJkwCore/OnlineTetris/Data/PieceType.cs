using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnlineTetris
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PieceType
    {
        /// <summary>
        /// <para>■■■■para>
        /// </summary>
        A,
        /// <summary>
        /// <para>□■□</para>
        /// <para>■■■</para>
        /// </summary>
        B,
        /// <summary>
        /// <para>■■</para>
        /// <para>■■</para>
        /// </summary>
        C,
        /// <summary>
        /// <para>□□■</para>
        /// <para>■■■</para>
        /// </summary>
        D,
        /// <summary>
        /// <para>■□□</para>
        /// <para>■■■</para>
        /// </summary>
        E,
        /// <summary>
        /// <para>□■■</para>
        /// <para>■■□</para>
        /// </summary>
        F,
        /// <summary>
        /// <para>■■□</para>
        /// <para>□■■</para>
        /// </summary>
        G,
    }
}
