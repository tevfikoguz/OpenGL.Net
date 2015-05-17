
// Copyright (C) 2013-2015 Luca Piccioni
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

using System;

namespace OpenGL
{
	/// <summary>
	/// Model matrix interface.
	/// </summary>
	public interface IModelMatrix : IMatrix4x4
	{
		#region Translation

		/// <summary>
		/// Get the translation of this ModelMatrix.
		/// </summary>
		Vertex4d Position { get; set; }

		/// <summary>
		/// Accumulate a translation on this model matrix.
		/// </summary>
		/// <param name="x">
		/// A <see cref="System.Double"/> indicating the translation on X axis.
		/// </param>
		/// <param name="y">
		/// A <see cref="System.Double"/> indicating the translation on Y axis.
		/// </param>
		void Translate(double x, double y);

		/// <summary>
		/// Accumulate a translation on this model matrix.
		/// </summary>
		/// <param name="x">
		/// A <see cref="System.Double"/> indicating the translation on X axis.
		/// </param>
		/// <param name="y">
		/// A <see cref="System.Double"/> indicating the translation on Y axis.
		/// </param>
		/// <param name="z">
		/// A <see cref="System.Double"/> indicating the translation on Z axis.
		/// </param>
		void Translate(double x, double y, double z);

		/// <summary>
		/// Accumulate a translation on this model matrix.
		/// </summary>
		/// <param name="p">
		/// A <see cref="Vertex2d"/> that specifies the translation.
		/// </param>
		void Translate(Vertex2d p);

		/// <summary>
		/// Accumulate a translation on this model matrix.
		/// </summary>
		/// <param name="p">
		/// A <see cref="Vertex3d"/> that specifies the translation.
		/// </param>
		void Translate(Vertex3d p);

		#endregion

		#region Rotation

		/// <summary>
		/// Get the rotation of this ModelMatrix.
		/// </summary>
		Quaternion Rotation { get; }

		/// <summary>
		/// Accumulate a rotation to this model matrix.
		/// </summary>
		/// <param name="q">
		/// A <see cref="Quaternion"/> representing the rotation.
		/// </param>
		void Rotate(Quaternion q);

		/// <summary>
		/// Accumulate a rotation around the X axis to this model matrix.
		/// </summary>
		/// <param name="angle">
		/// A <see cref="System.Double"/> representing the rotation around the X axis.
		/// </param>
		void RotateX(double angle);

		/// <summary>
		/// Accumulate a rotation around the X axis to this model matrix.
		/// </summary>
		/// <param name="angle">
		/// A <see cref="System.Double"/> representing the rotation around the X axis.
		/// </param>
		void RotateY(double angle);

		/// <summary>
		/// Accumulate a rotation around the X axis to this model matrix.
		/// </summary>
		/// <param name="angle">
		/// A <see cref="System.Double"/> representing the rotation around the X axis.
		/// </param>
		void RotateZ(double angle);

		#endregion

		#region Scale

		/// <summary>
		/// Accumulate a scaling to this model matrix.
		/// </summary>
		/// <param name="s">
		/// A <see cref="System.Double"/> holding the scaling factor on X, Y and Z axes.
		/// </param>
		void Scale(double s);

		/// <summary>
		/// Accumulate a scaling to this model matrix.
		/// </summary>
		/// <param name="x">
		/// A <see cref="System.Double"/> holding the scaling factor on X axis.
		/// </param>
		/// <param name="y">
		/// A <see cref="System.Double"/> holding the scaling factor on Y axis.
		/// </param>
		void Scale(double x, double y);

		/// <summary>
		/// Accumulate a scaling to this model matrix.
		/// </summary>
		/// <param name="x">
		/// A <see cref="System.Double"/> holding the scaling factor on X axis.
		/// </param>
		/// <param name="y">
		/// A <see cref="System.Double"/> holding the scaling factor on Y axis.
		/// </param>
		/// <param name="z">
		/// A <see cref="System.Double"/> holding the scaling factor on Z axis.
		/// </param>
		void Scale(double x, double y, double z);

		/// <summary>
		/// Accumulate a scaling to this model matrix.
		/// </summary>
		/// <param name="s">
		/// A <see cref="Vertex2d"/> holding the scaling factors on three dimensions.
		/// </param>
		void Scale(Vertex2d s);

		/// <summary>
		/// Accumulate a scaling to this model matrix.
		/// </summary>
		/// <param name="s">
		/// A <see cref="Vertex3d"/> holding the scaling factors on three dimensions.
		/// </param>
		void Scale(Vertex3d s);

		#endregion

		#region Look At

		/// <summary>
		/// Get/set the forward vector of this view matrix.
		/// </summary>
		Vertex3d ForwardVector { get; set; }

		/// <summary>
		/// Get the right vector of this view matrix.
		/// </summary>
		Vertex3d RightVector { get; set; }

		/// <summary>
		/// Get/set the up vector of this view matrix.
		/// </summary>
		Vertex3d UpVector { get; set; }

		/// <summary>
		/// Compute a model-view matrix in order to simulate the gluLookAt mithical GLU routine.
		/// </summary>
		/// <param name="targetPosition">
		/// A <see cref="Vertex3d"/> that specify the eye target position, in local coordinates.
		/// </param>
		/// <remarks>
		/// It returns a view transformation matrix used to transform the world coordinate, in order to view
		/// the world from current position, looking at <paramref name="targetPosition"/> having
		/// an up direction equal to the current up vector.
		/// </remarks>
		void LookAtTarget(Vertex3d targetPosition);

		/// <summary>
		/// Compute a model-view matrix in order to simulate the gluLookAt mithical GLU routine.
		/// </summary>
		/// <param name="eyePosition">
		/// A <see cref="Vertex3d"/> that specify the eye position, in local coordinates.
		/// </param>
		/// <param name="targetPosition">
		/// A <see cref="Vertex3d"/> that specify the eye target position, in local coordinates.
		/// </param>
		/// <remarks>
		/// It returns a view transformation matrix used to transform the world coordinate, in order to view
		/// the world from <paramref name="eyePosition"/>, looking at <paramref name="targetPosition"/> having
		/// an up direction equal to the current up vector.
		/// </remarks>
		void LookAtTarget(Vertex3d eyePosition, Vertex3d targetPosition);

		/// <summary>
		/// Compute a model-view matrix in order to simulate the gluLookAt mithical GLU routine.
		/// </summary>
		/// <param name="eyePosition">
		/// A <see cref="Vertex3d"/> that specify the eye position, in local coordinates.
		/// </param>
		/// <param name="targetPosition">
		/// A <see cref="Vertex3d"/> that specify the eye target position, in local coordinates.
		/// </param>
		/// <param name="upVector">
		/// A <see cref="Vertex3d"/> that specify the up vector of the view camera abstraction.
		/// </param>
		/// <returns>
		/// It returns a view transformation matrix used to transform the world coordinate, in order to view
		/// the world from <paramref name="eyePosition"/>, looking at <paramref name="targetPosition"/> having
		/// an up direction equal to <paramref name="upVector"/>.
		/// </returns>
		void LookAtTarget(Vertex3d eyePosition, Vertex3d targetPosition, Vertex3d upVector);

		/// <summary>
		/// Setup this model matrix to view the universe in a certain direction.
		/// </summary>
		/// <param name="eyePosition">
		/// A <see cref="Vertex3d"/> that specify the eye position, in local coordinates.
		/// </param>
		/// <param name="forwardVector">
		/// A <see cref="Vertex3d"/> that specify the direction of the view. It will be normalized.
		/// </param>
		/// <param name="upVector">
		/// A <see cref="Vertex3d"/> that specify the up vector of the view camera abstraction. It will be normalized
		/// </param>
		/// <returns>
		/// It returns a view transformation matrix used to transform the world coordinate, in order to view
		/// the world from <paramref name="eyePosition"/>, looking at <paramref name="forwardVector"/> having
		/// an up direction equal to <paramref name="upVector"/>.
		/// </returns>
		void LookAtDirection(Vertex3d eyePosition, Vertex3d forwardVector, Vertex3d upVector);

		#endregion

		#region Square Matrix

		/// <summary>
		/// Inverse Matrix of this Matrix.
		/// </summary>
		/// <returns>
		/// A <see cref="IMatrix"/> representing the inverse matrix of this Matrix.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// The exception is thrown if this Matrix is not square, or it's determinant is 0.0.
		/// </exception>
		new IModelMatrix GetInverseMatrix();

		#endregion

		#region Methods

		/// <summary>
		/// Multiply this IModelMatrix with another IModelMatrix.
		/// </summary>
		/// <param name="a">
		/// A <see cref="IModelMatrix"/> that specifies the right multiplication operand.
		/// </param>
		/// <returns>
		/// A <see cref="IModelMatrix"/> resulting from the product of this matrix and the matrix <paramref name="a"/>.
		/// </returns>
		IModelMatrix Multiply(IModelMatrix a);

		/// <summary>
		/// Compute the transpose of this model matrix.
		/// </summary>
		/// <returns>
		/// A <see cref="IModelMatrix"/> which hold the transpose of this model matrix.
		/// </returns>
		new IModelMatrix Transpose();

		#endregion
	}
}
