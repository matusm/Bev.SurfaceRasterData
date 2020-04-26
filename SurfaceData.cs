using System;
using System.Collections.Generic;

namespace Bev.SurfaceRasterData
{
    public class SurfaceData
    {

        private readonly double[,] zValues;
        private int runningIndex;
        private const double invalidValue = double.NaN;

        #region Ctor
        public SurfaceData(int numPoints, int numProfiles)
        {
            NumberOfPointsPerProfile = numPoints;
            NumberOfProfiles = numProfiles;
            zValues = new double[NumberOfPointsPerProfile, NumberOfProfiles];
            SetPropertiesToDefault();
            ClearData(double.NaN);
        }
        #endregion

        #region Properties
        public bool IsDataComplete => (runningIndex >= NumberOfPointsPerProfile * NumberOfProfiles);
        public int NumberOfPointsPerProfile { get; private set; } // NumPoints
        public int NumberOfProfiles { get; private set; } // NumProfiles
        public double XScale { get; set; }
        public double YScale { get; set; }
        // following properties as proposed by Xiangqi Lan (2014)
        public double ZScale { get; set; }
        public string XUnit { get; set; }
        public string YUnit { get; set; }
        public string ZUnit { get; set; }
        public double XOffset { get; set; }
        public double YOffset { get; set; }
        public double ZOffset { get; set; }
        public double ScanFieldDimensionX => XScale * (NumberOfPointsPerProfile - 1);
        public double ScanFieldDimensionY => YScale * (NumberOfProfiles - 1);
        public bool IsMetricUnit { get; set; }
        public string Description { get; set; }
        public string Manufacturer { get; set; }
        public string VersionField { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public Dictionary<string, string> MetaData { get; set; }
        #endregion

        #region Methods

        public void ClearData(double value)
        {
            ResetRunningIndex();
            for (int i = 0; i < NumberOfPointsPerProfile; i++)
            {
                for (int j = 0; j < NumberOfProfiles; j++)
                {
                    zValues[i, j] = value;
                }
            }
        }

        // the hight data is filled up point by point in the raster data array
        // this is a slow process but compatible with usual file parsing techniques
        public void FillUpData(double value)
        {
            int pointsIndex = runningIndex % NumberOfPointsPerProfile;
            if (pointsIndex >= NumberOfPointsPerProfile)
                return;
            int profileIndex = runningIndex / NumberOfPointsPerProfile;
            if (profileIndex >= NumberOfProfiles)
                return;
            zValues[pointsIndex, profileIndex] = value;
            runningIndex++;
        }

        public double[] GetProfileFor(int profileIndex)
        {
            double[] profile = new double[NumberOfPointsPerProfile];
            for (int i = 0; i < NumberOfPointsPerProfile; i++)
            {
                // this assures that NaN array is returned for a profile index outside the range
                profile[i] = GetValueFor(i, profileIndex);
            }
            return profile;
        }

        public Point3D[] GetPointsProfileFor(int profileIndex)
        {
            Point3D[] profile = new Point3D[NumberOfPointsPerProfile];
            for (int i = 0; i < NumberOfPointsPerProfile; i++)
            {
                // this assures that NaN array is returned for a profile index outside the range
                profile[i] = GetPointFor(i, profileIndex);
            }
            return profile;
        }

        public double GetValueFor(int pointIndex, int profileIndex)
        {
            if (pointIndex < 0) return invalidValue;
            if (profileIndex < 0) return invalidValue;
            if (pointIndex >= NumberOfPointsPerProfile) return invalidValue;
            if (profileIndex >= NumberOfProfiles) return invalidValue;
            return ZOffset + zValues[pointIndex, profileIndex];
        }

        public Point3D GetPointFor(int pointIndex, int profileIndex)
        {
            double xCoordinate = XOffset + pointIndex * XScale;
            double yCoordinate = YOffset + profileIndex * YScale;
            if (pointIndex < 0) xCoordinate = invalidValue;
            if (profileIndex < 0) yCoordinate = invalidValue;
            if (pointIndex >= NumberOfPointsPerProfile) xCoordinate = invalidValue;
            if (profileIndex >= NumberOfProfiles) yCoordinate = invalidValue;
            return new Point3D(xCoordinate, yCoordinate, GetValueFor(pointIndex, profileIndex));
        }

        #endregion

        #region Private stuff

        private void ResetRunningIndex()
        {
            runningIndex = 0;
        }

        private void SetPropertiesToDefault()
        {
            XScale = 1.0;
            YScale = 1.0;
            ZScale = 1.0;
            XUnit = "m";
            YUnit = "m";
            ZUnit = "m";
            XOffset = 0.0;
            YOffset = 0.0;
            ZOffset = 0.0;
            IsMetricUnit = true;
            CreateDate = null;
            ModifyDate = null;
        }

        #endregion
    }
}
