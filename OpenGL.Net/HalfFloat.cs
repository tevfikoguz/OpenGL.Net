
// Copyright (C) 2011-2015 Luca Piccioni
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
// USA

// Note
// The implementation is based on OpenTK.Half implementation

using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace OpenGL
{
	/// <summary>
	/// HalfFloat floating-point number (16 bit).
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct HalfFloat : ISerializable, IComparable<HalfFloat>, IFormattable, IEquatable<HalfFloat>
	{
		#region Constructors

		 /// <summary>
		/// Constuct a HalfFloat by specifying its value.
		/// </summary>
		/// <param name="f">
		/// Floating-point (single precision) number.
		/// </param>
		public HalfFloat(Single f) : this()
		{
			unsafe {
				mBits = FloatToHalf(*(int*)&f);
			}
		}

		/// <summary>
		/// Constuct a HalfFloat by specifying its value.
		/// </summary>
		/// <param name="f">
		/// Floating-point (single precision) number.
		/// </param>
		/// <param name="throwOnError">
		/// Enable checks that will throw if the conversion result is not meaningful.
		/// </param>
		/// <exception cref="ArithmeticException">
		/// Exception thrown if one of the following condition is satisfied:
		/// - <paramref name="f"/> is greater than MaxValue.
		/// - <paramref name="f"/> is lesser than -MaxValue.
		/// - <paramref name="f"/> is Nan.
		/// - <paramref name="f"/> is positive infinity.
		/// - <paramref name="f"/> is negative infinity.
		/// </exception>
		public HalfFloat(Single f, bool throwOnError) : this(f)
		{
			if (throwOnError) {
				if (f > +HalfFloat.MaxValue)
					throw new ArithmeticException("positive maximum value exceeded.");
				if (f < -HalfFloat.MaxValue)
					throw new ArithmeticException("negative minimum value exceeded.");
				if (Single.IsNaN(f) == true)
					throw new ArithmeticException("not a number (NaN).");
				if (Single.IsPositiveInfinity(f) == true)
					throw new ArithmeticException("positive infinity.");
				if (Single.IsNegativeInfinity(f) == true)
					throw new ArithmeticException("negative infinity.");
			}
		}

		/// <summary>
		/// Constuct a HalfFloat by specifying its value.
		/// </summary>
		/// <param name="d">
		/// Floating-point (double precision) number.
		/// </param>
		public HalfFloat(Double d) : this((Single)d)
		{
			
		}

		/// <summary>
		/// Constuct a HalfFloat by specifying its value.
		/// </summary>
		/// <param name="d">
		/// Floating-point (double precision) number.
		/// </param>
		/// <param name="throwOnError">
		/// Enable checks that will throw if the conversion result is not meaningful.
		/// </param>
		/// <exception cref="ArithmeticException">
		/// Exception thrown if one of the following condition is satisfied:
		/// - <paramref name="d"/> is greater than MaxValue.
		/// - <paramref name="d"/> is lesser than -MaxValue.
		/// </exception>
		public HalfFloat(Double d, bool throwOnError) : this((Single)d, throwOnError) { }

		#endregion

		#region Structure Storage

		/// <summary>
		/// The half-float mBits.
		/// </summary>
		private UInt16 mBits;

		#endregion

		#region Data Type Conversions

		/// <summary>
		/// Converts the 16-bit half to 32-bit floating-point.
		/// </summary>
		/// <returns>
		/// A single-precision floating-point number equivalent to this HalfFloat.
		/// </returns>
		public Single ToSingle()
		{
			int i = HalfToFloat(mBits);

			unsafe {
				return *(float*)&i;
			}
		}

		/// <summary>
		/// Converts the 16-bit half to 32-bit floating-point.
		/// </summary>
		/// <param name="ui16">
		/// A <see cref="System.UInt16"/> that specifies the bit representation of the half-precision
		/// floating-point.
		/// </param>
		/// <returns>
		/// A single-precision floating-point number equivalent to <paramref name="ui16"/>.
		/// </returns>
		public static Single ToFloat(UInt16 ui16)
		{
			unsafe {
				int i = HalfToFloat(ui16);

				return (*(float*)&i);
			}
		}

		/// <summary>
		/// Ported from OpenEXR's IlmBase 1.0.1
		/// </summary>
		public static Int32 HalfToFloat(UInt16 ui16)
		{
			Int32 sign = (ui16 >> 15) & 0x00000001;
			Int32 exponent = (ui16 >> 10) & 0x0000001f;
			Int32 mantissa = ui16 & 0x000003ff;

			if (exponent == 0)
			{
				if (mantissa == 0)
				{
					// Plus or minus zero

					return sign << 31;
				}
				else
				{
					// Denormalized number -- renormalize it

					while ((mantissa & 0x00000400) == 0)
					{
						mantissa <<= 1;
						exponent -= 1;
					}

					exponent += 1;
					mantissa &= ~0x00000400;
				}
			}
			else if (exponent == 31)
			{
				if (mantissa == 0)
				{
					// Positive or negative infinity

					return (sign << 31) | 0x7f800000;
				}
				else
				{
					// Nan -- preserve sign and significand mBits

					return (sign << 31) | 0x7f800000 | (mantissa << 13);
				}
			}

			// Normalized number

			exponent = exponent + (127 - 15);
			mantissa = mantissa << 13;

			// Assemble S, E and M.

			return (sign << 31) | (exponent << 23) | mantissa;
		}

		/// <summary>
		/// Ported from OpenEXR's IlmBase 1.0.1
		/// </summary>
		public static UInt16 FloatToHalf(Int32 si32)
		{
			// Our floating point number, F, is represented by the bit pattern in integer i.
			// Disassemble that bit pattern into the sign, S, the exponent, E, and the significand, M.
			// Shift S into the position where it will go in in the resulting half number.
			// Adjust E, accounting for the different exponent bias of float and half (127 versus 15).

			Int32 sign = (si32 >> 16) & 0x00008000;
			Int32 exponent = ((si32 >> 23) & 0x000000ff) - (127 - 15);
			Int32 mantissa = si32 & 0x007fffff;

			// Now reassemble S, E and M into a half:

			if (exponent <= 0) {
				if (exponent < -10) {
					// E is less than -10. The absolute value of F is less than HalfFloat.MinValue
					// (F may be a small normalized float, a denormalized float or a zero).
					//
					// We convert F to a half zero with the same sign as F.

					return (UInt16)sign;
				}

				// E is between -10 and 0. F is a normalized float whose magnitude is less than HalfFloat.MinNormalizedValue.
				//
				// We convert F to a denormalized half.

				// Add an explicit leading 1 to the significand.

				mantissa = mantissa | 0x00800000;

				// Round to M to the nearest (10+E)-bit value (with E between -10 and 0); in case of a tie, round to the nearest even value.
				//
				// Rounding may cause the significand to overflow and make our number normalized. Because of the way a half's bits
				// are laid out, we don't have to treat this case separately; the code below will handle it correctly.

				Int32 t = 14 - exponent;
				Int32 a = (1 << (t - 1)) - 1;
				Int32 b = (mantissa >> t) & 1;

				mantissa = (mantissa + a + b) >> t;

				// Assemble the half from S, E (==zero) and M.

				return (UInt16)(sign | mantissa);
			} else if (exponent == 0xff - (127 - 15)) {
				if (mantissa == 0) {
					// F is an infinity; convert F to a half infinity with the same sign as F.

					return (UInt16)(sign | 0x7c00);
				} else {
					// F is a NAN; we produce a half NAN that preserves the sign bit and the 10 leftmost bits of the
					// significand of F, with one exception: If the 10 leftmost bits are all zero, the NAN would turn 
					// into an infinity, so we have to set at least one bit in the significand.

					mantissa >>= 13;
					return (UInt16)(sign | 0x7c00 | mantissa | ((mantissa == 0) ? 1 : 0));
				}
			} else {
				// E is greater than zero.  F is a normalized float. We try to convert F to a normalized half.

				// Round to M to the nearest 10-bit value. In case of a tie, round to the nearest even value.

				mantissa = mantissa + 0x00000fff + ((mantissa >> 13) & 1);

				if ((mantissa & 0x00800000) == 1) {
					mantissa = 0;		// overflow in significand,
					exponent += 1;		// adjust exponent
				}

				// exponent overflow
				if (exponent > 30) throw new ArithmeticException("HalfFloat: Hardware floating-point overflow.");

				// Assemble the half from S, E and M.

				return (UInt16)(sign | (exponent << 10) | (mantissa >> 13));
			}
		}

		#endregion

		#region Cast Operators

		/// <summary>
		/// Converts a <see cref="System.Single"/> to a HalfFloat.
		/// </summary>
		/// <param name="f">
		/// A <see cref="System.Single"/> to convert.
		/// </param>
		/// <returns>
		/// A <see cref="HalfFloat"/> corresponding to <paramref name="f"/>.
		/// </returns>
		public static explicit operator HalfFloat(float f)
		{
			return new HalfFloat(f);
		}

		/// <summary>
		/// Converts a <see cref="System.Double"/> to a HalfFloat.
		/// </summary>
		/// <param name="d">
		/// A <see cref="System.Double"/> to convert.
		/// </param>
		/// <returns>
		/// A <see cref="HalfFloat"/> corresponding to <paramref name="d"/>.
		/// </returns>
		public static explicit operator HalfFloat(double d)
		{
			return new HalfFloat(d);
		}

		/// <summary>
		/// Converts a HalfFloat to a <see cref="System.Single"/>.
		/// </summary>
		/// <param name="h">
		/// A <see cref="HalfFloat"/> to convert.
		/// </param>
		/// <returns>
		/// A <see cref="System.Single"/> corresponding to <paramref name="h"/>.
		/// </returns>
		public static implicit operator float(HalfFloat h)
		{
			return h.ToSingle();
		}

		/// <summary>
		/// Converts a HalfFloat to a <see cref="System.Double"/>.
		/// </summary>
		/// <param name="h">
		/// A <see cref="HalfFloat"/> to convert.
		/// </param>
		/// <returns>
		/// A <see cref="System.Double"/> corresponding to <paramref name="h"/>.
		/// </returns>
		public static implicit operator double(HalfFloat h)
		{
			return ((double)h.ToSingle());
		}

		#endregion

		#region Constants

		/// <summary>
		/// Smallest positive value represented by a HalfFloat.
		/// </summary>
		public const Single MinValue = 5.96046448e-08f;

		/// <summary>
		/// Smallest positive (normalized) value represented by a HalfFloat.
		/// </summary>
		public const Single MinNormalizedValue = 6.10351562e-05f;

		/// <summary>
		/// Largest positive value represented by a HalfFloat.
		/// </summary>
		public const Single MaxValue = 65504.0f;

		/// <summary>
		/// Smallest positive value 'e' for which half(1.0 + e) != half(1.0).
		/// </summary>
		public const Single Epsilon = 0.00097656f;

		#endregion

		#region Special Value Check

		/// <summary>
		/// Returns a boolean value indicating whether this half-float represents zero.
		/// </summary>
		public bool IsZero { get { return (mBits == 0) || (mBits == 0x8000); } }

		/// <summary>
		/// Returns a boolean value indicating whether this half-float represents NaN (not a number).
		/// </summary>
		public bool IsNaN { get { return (((mBits & 0x7C00) == 0x7C00) && (mBits & 0x03FF) != 0x0000); } }

		/// <summary>
		/// Returns a boolean value indicating whether this half-float represents a positive infinite value.
		/// </summary>
		public bool IsPositiveInfinity { get { return (mBits == 31744); } }

		/// <summary>
		/// Returns a boolean value indicating whether this half-float represents a negative infinite value.
		/// </summary>
		public bool IsNegativeInfinity { get { return (mBits == 64512); } }

		#endregion

		#region String Parser

		/// <summary>
		/// Converts the string representation of a number to a HalfFloat.
		/// </summary>
		/// <param name="s">
		/// String representation of the number to convert.
		/// </param>
		/// <returns>
		/// A HalfFloat instance which value is represented by <paramref name="s"/>.
		/// </returns>
		public static HalfFloat Parse(string s)
		{
			return ((HalfFloat)Single.Parse(s));
		}

		/// <summary>
		/// Converts the string representation of a number to a HalfFloat.
		/// </summary>
		/// <param name="s">
		/// String representation of the number to convert.
		/// </param>
		/// <param name="style">
		/// Specifies the format of <paramref name="s"/>.
		/// </param>
		/// <param name="provider">
		/// Culture-specific formatting information used in string parsing.
		/// </param>
		/// <returns>
		/// A HalfFloat instance which value is represented by <paramref name="s"/>.
		/// </returns>
		public static HalfFloat Parse(string s, System.Globalization.NumberStyles style, IFormatProvider provider)
		{
			return (HalfFloat)Single.Parse(s, style, provider);
		}

		/// <summary>
		/// Converts the string representation of a number to a HalfFloat.
		/// </summary>
		/// <param name="s">
		/// String representation of the number to convert.
		/// </param>
		/// <param name="result">
		/// The parsed HalfFloat value.
		/// </param>
		/// <returns>
		/// It returns a boolean value indicating whether <paramref name="s"/> was parsed correctly to HalfFloat.
		/// </returns>
		public static bool TryParse(string s, out HalfFloat result)
		{
			float f;
			bool parsed;

			// Single.TryPause implementation
			parsed = Single.TryParse(s, out f);
			// Store result all the same
			result = (HalfFloat)f;

			return (parsed);
		}

		/// <summary>
		/// Converts the string representation of a number to a HalfFloat.
		/// </summary>
		/// <param name="s">
		/// String representation of the number to convert.
		/// </param>
		/// <param name="style">
		/// Specifies the format of <paramref name="s"/>.
		/// </param>
		/// <param name="provider">
		/// Culture-specific formatting information used in string parsing.
		/// </param>
		/// <param name="result">
		/// The parsed HalfFloat value.
		/// </param>
		/// <returns>
		/// It returns a boolean value indicating whether <paramref name="s"/> was parsed correctly to HalfFloat.
		/// </returns>
		public static bool TryParse(string s, System.Globalization.NumberStyles style, IFormatProvider provider, out HalfFloat result)
		{
			float f;
			bool parsed;

			// Single.TryPause implementation
			parsed = Single.TryParse(s, style, provider, out f);
			// Store result all the same
			result = (HalfFloat)f;

			return (parsed);
		}

		#endregion

		#region ISerializable Implementation

		/// <summary>
		/// Constructor used by ISerializable to deserialize the object.
		/// </summary>
		/// <param name="info">
		/// </param>
		/// <param name="context">
		/// </param>
		public HalfFloat(SerializationInfo info, StreamingContext context)
		{
			mBits = (ushort)info.GetValue("HalfFloat.mBits", typeof(ushort));
		}

		/// <summary>
		/// Used by ISerialize to serialize the object.
		/// </summary>
		/// <param name="info">
		/// </param>
		/// <param name="context">
		/// </param>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("HalfFloat.mBits", mBits);
		}

		#endregion

		#region IEquatable<HalfFloat> Implementation

		/// <summary>
		/// Returns a boolean value indicating whether this instance is equal to <paramref name="other"/>.
		/// </summary>
		/// <param name="other">
		/// The HalfFloat to be compared with this HalfFloat.
		/// </param>
		/// <returns>
		/// It returns a boolean value indicating whether <paramref name="other"/> euquals dst this instance.
		/// </returns>
		public bool Equals(HalfFloat other)
		{
			const int maxUlps = 1;

			short aInt, bInt;
			unchecked { aInt = (short)other.mBits; }
			unchecked { bInt = (short)this.mBits; }

			// Make aInt lexicographically ordered as a twos-complement int
			if (aInt < 0)
				aInt = (short)(0x8000 - aInt);

			// Make bInt lexicographically ordered as a twos-complement int
			if (bInt < 0)
				bInt = (short)(0x8000 - bInt);

			short intDiff = System.Math.Abs((short)(aInt - bInt));

			if (intDiff <= maxUlps)
				return true;

			return false;
		}

		#endregion

		#region IComparable<HalfFloat> Implementation

		/// <summary>
		/// Compares this instance to a specified half-precision floating-point number
		/// and returns an integer that indicates whether the value of this instance
		/// is less than, equal to, or greater than the value of the specified half-precision
		/// floating-point number. 
		/// </summary>
		/// <param name="other">A half-precision floating-point number to compare.</param>
		/// <returns>
		/// </returns>
		public int CompareTo(HalfFloat other)
		{
			return ((float)this).CompareTo((float)other);
		}

		#endregion

		#region IFormattable Implementation

		///<summary>
		/// Converts this HalfFloat into a human-legible string representation.
		///</summary>
		///<returns>
		///The string representation of this instance.
		///</returns>
		public override string ToString()
		{
			return this.ToSingle().ToString();
		}

		///<summary>
		/// Converts this HalfFloat into a human-legible string representation.
		///</summary>
		///<param name="format">
		/// Formatting for the output string.
		///</param>
		///<param name="formatProvider">
		/// Culture-specific formatting information.
		///</param>
		///<returns>
		///The string representation of this instance.
		///</returns>
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return this.ToSingle().ToString(format, formatProvider);
		}

		#endregion
	}
}