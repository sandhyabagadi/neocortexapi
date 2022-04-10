﻿// Copyright (c) Damir Dobric. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoCortex;
using NeoCortexApi;
using NeoCortexApi.Entities;
using NeoCortexApi.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTestsProject
{
    [TestClass]
    public class SpatialPoolerBumpUpWeakColumnsTest
    {
        private SpatialPooler sp;
        private Connections mem;

        /// <summary>
        /// create htmconfig with default parameters required for the unit tests 
        /// and also create connection instance for spatial pooler intialization
        /// </summary>
        private void InitTestSPInstance()
        {
            var htmConfig = new HtmConfig(new int[] { 10 }, new int[] { 5 })
            {
                PotentialRadius = 5,
                PotentialPct = 0.5,
                GlobalInhibition = true,
                LocalAreaDensity = -1,
                NumActiveColumnsPerInhArea = 3,
                StimulusThreshold = 0.0,
                SynPermActiveInc = 0.1,
                SynPermInactiveDec = 0.01,
                SynPermConnected = 0.1,
                MinPctActiveDutyCycles = 0.1,
                MinPctOverlapDutyCycles = 0.1,
                DutyCyclePeriod = 10,
                MaxBoost = 10,
                Random = new ThreadSafeRandom(42),
            };

            mem = new Connections(htmConfig);
            sp = new SpatialPoolerMT();
            sp.Init(mem);
        }


        /// <summary>
        /// It makes sure that Testing permanence values are updated correctly in BumpUpWeakColumns method with SynPermBelowStimulusInc as 0.1
        /// This test ensures that weak columns ( OverlapDutyCycles lessthan MinOverlapDutyCycles) permanence values are increased correctly.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Prod")]
        public void testBumpUpWeakColumns_1()
        {

            InitTestSPInstance();

            mem.HtmConfig.SynPermBelowStimulusInc = 0.01;
            mem.HtmConfig.SynPermTrimThreshold = 0.05;
            mem.HtmConfig.OverlapDutyCycles = new double[] { 0, 0.009, 0.1, 0.001, 0.002 };
            mem.HtmConfig.MinOverlapDutyCycles = new double[] { .01, .01, .01, .01, .01 };


            // An array of permanence values for a column. The array is "sparse", i.e. it contains an entry for each input bit, even if the permanence value is 0. 
            int[][] testingPools = new int[][] {
                new int[] { 1, 1, 1, 1, 0, 0, 0, 0, 1, 1 },
                new int[] { 1, 0, 0, 0, 1, 1, 0, 1, 1, 1 },
                new int[] { 0, 0, 1, 0, 1, 1, 1, 0, 0, 0 },
                new int[] { 1, 1, 1, 0, 0, 0, 1, 0, 0, 0 },
                new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
            };

            // Testing permanence values for spatialpooler bumpup weak columns method
            double[][] testingpermanences = new double[][] {
                new double[] { 0.200, 0.120, 0.090, 0.040, 0.000, 0.000, 0.000, 0.000,0.300,0.400 },
                new double[] { 0.150, 0.000, 0.000, 0.000, 0.180, 0.120, 0.000, 0.450,0.160,0.190 },
                new double[] { 0.000, 0.000, 0.014, 0.000, 0.032, 0.044, 0.110, 0.000,0.000,0.000 },
                new double[] { 0.041, 0.000, 0.000, 0.000, 0.000, 0.000, 0.178, 0.000,0.000,0.000 },
                new double[] { 0.100, 0.738, 0.045, 0.002, 0.050, 0.008, 0.208, 0.034,0.200,0.300 }
            };

            // Expected permanence values calculated manually using SynPermTrimThreshold and SynPermBelowStimulusInc when OverlapDutyCycles < MinOverlapDutyCycles
            double[][] ExpectedPermanences = new double[][] {
            new double[] { 0.210, 0.130, 0.100, 0.000, 0.000, 0.000, 0.000, 0.000, 0.310, 0.410 },
            new double[] { 0.160, 0.000, 0.000, 0.000, 0.190, 0.130, 0.000, 0.460, 0.170, 0.200 },
            new double[] { 0.000, 0.000, 0.014, 0.000, 0.032, 0.044, 0.110, 0.000, 0.000, 0.000 },
            new double[]  { 0.051, 0.000, 0.000, 0.000, 0.000, 0.000, 0.188,0.000, 0.000, 0.000 },
            new double[] { 0.110, 0.748, 0.055, 0.000, 0.060, 0.000, 0.218, 0.000, 0.210, 0.310 }
        };


            for (int i = 0; i < mem.HtmConfig.NumColumns; i++)
            {
                int[] indexes = ArrayUtils.IndexWhere(testingPools[i], n => n == 1);

                mem.GetColumn(i).SetProximalConnectedSynapsesForTest(mem, indexes);
                mem.GetColumn(i).SetPermanences(mem.HtmConfig, testingpermanences[i]);
            }

            //Execute method being tested
            sp.BumpUpWeakColumns(mem);

            for (int i = 0; i < mem.HtmConfig.NumColumns; i++)
            {
                // calculated permanence values from the spatialpooler BumpUpWeakColumns method
                double[] calculatedperms = mem.GetColumn(i).ProximalDendrite.RFPool.GetDensePermanences(mem.HtmConfig.NumInputs);
                for (int j = 0; j < ExpectedPermanences[i].Length; j++)
                {
                    // Veriying absolute values of manually calculated permanence and permanence values from BumpUpWeakColumns method
                    Assert.IsTrue(Math.Abs(ExpectedPermanences[i][j] - calculatedperms[j]) <= 0.01);
                }
            }
        }

        /// <summary>
        /// It makes sure that Testing permanence values are updated correctly in BumpUpWeakColumns method with SynPermBelowStimulusInc as 0.01
        /// This test ensures that weak columns ( OverlapDutyCycles lessthan MinOverlapDutyCycles) permanence values are increased correctly.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Prod")]
        public void testBumpUpWeakColumns_2()
        {
            InitTestSPInstance();

            mem.HtmConfig.SynPermBelowStimulusInc = 0.02;
            mem.HtmConfig.SynPermTrimThreshold = 0.01;
            mem.HtmConfig.OverlapDutyCycles = new double[] { 0, 0.009, 0.1, 0.001, 0.002 };
            mem.HtmConfig.MinOverlapDutyCycles = new double[] { .01, .01, .01, .01, .01 };


            // An array of permanence values for a column. The array is "sparse", i.e. it contains an entry for each input bit, even if the permanence value is 0. 
            int[][] testingPools = new int[][] {
                new int[] { 1, 1, 1, 1, 0, 0, 0, 0, 1, 1 },
                new int[] { 1, 0, 0, 0, 1, 1, 0, 1, 1, 1 },
                new int[] { 0, 0, 1, 0, 1, 1, 1, 0, 0, 0 },
                new int[] { 1, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
                new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
            };

            // Testing permanence values for spatialpooler bumpup weak columns method
            double[][] testingpermanences = new double[][] {
                new double[] { 0.200, 0.120, 0.090, 0.040, 0.000, 0.000, 0.000, 0.000,0.300,0.400 },
                new double[] { 0.150, 0.000, 0.000, 0.000, 0.180, 0.120, 0.000, 0.450,0.160,0.190 },
                new double[] { 0.000, 0.000, 0.014, 0.000, 0.032, 0.044, 0.110, 0.000,0.000,0.000 },
                new double[] { 0.041, 0.000, 0.000, 0.000, 0.000, 0.000, 0.178, 0.000,0.000,0.000 },
                new double[] { 0.100, 0.738, 0.045, 0.002, 0.050, 0.008, 0.208, 0.034,0.200,0.300 }
            };

            // Expected permanence values calculated manually using SynPermTrimThreshold and SynPermBelowStimulusInc when OverlapDutyCycles < MinOverlapDutyCycles
            double[][] ExpectedPermanences = new double[][] {
            new double[] { 0.220, 0.140, 0.110, 0.060, 0.000, 0.000, 0.000, 0.000, 0.320, 0.420 },
            new double[] { 0.170, 0.000, 0.000, 0.000, 0.200, 0.140, 0.000, 0.470, 0.180, 0.210 },
            new double[] { 0.000, 0.000, 0.014, 0.000, 0.032, 0.044, 0.110, 0.000, 0.000, 0.000 },
            new double[]  { 0.061, 0.000, 0.000, 0.000, 0.000, 0.000, 0.198,0.000, 0.000, 0.000 },
            new double[] { 0.120, 0.758, 0.065, 0.022, 0.070, 0.028, 0.228, 0.054, 0.220, 0.320 }
        };


            for (int i = 0; i < mem.HtmConfig.NumColumns; i++)
            {
                int[] indexes = ArrayUtils.IndexWhere(testingPools[i], n => n == 1);

                mem.GetColumn(i).SetProximalConnectedSynapsesForTest(mem, indexes);
                mem.GetColumn(i).SetPermanences(mem.HtmConfig, testingpermanences[i]);
            }

            //Execute method being tested
            sp.BumpUpWeakColumns(mem);

            for (int i = 0; i < mem.HtmConfig.NumColumns; i++)
            {
                // calculated permanence values from the spatialpooler BumpUpWeakColumns method
                double[] calculatedperms = mem.GetColumn(i).ProximalDendrite.RFPool.GetDensePermanences(mem.HtmConfig.NumInputs);
                for (int j = 0; j < ExpectedPermanences[i].Length; j++)
                {
                    // Veriying absolute values of manually calculated permanence and permanence values from BumpUpWeakColumns method
                    Assert.IsTrue(Math.Abs(ExpectedPermanences[i][j] - calculatedperms[j]) <= 0.01);
                }
            }
        }
        /**
         * Testing permanence values are not updated  when All OverlapDutyCycles are greater than MinOverlapDutyCycles
         */
        /// <summary>
        /// It makes sure that Testing permanence values are not updated when All OverlapDutyCycles are greater than MinOverlapDutyCycles
        /// This test ensures that weak columns ( OverlapDutyCycles lessthan MinOverlapDutyCycles) permanence values are only incremented but not the active columns permanence values.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Prod")]
        public void testBumpUpWeakColumns_3()
        {

            InitTestSPInstance();

            mem.HtmConfig.SynPermBelowStimulusInc = 0.02;
            mem.HtmConfig.SynPermTrimThreshold = 0.01;
            mem.HtmConfig.OverlapDutyCycles = new double[] { 0.1, 0.1, 0.1, 0.1, 0.1 };
            mem.HtmConfig.MinOverlapDutyCycles = new double[] { .01, .01, .01, .01, .01 };


            // An array of permanence values for a column. The array is "sparse", i.e. it contains an entry for each input bit, even if the permanence value is 0. 
            int[][] testingPools = new int[][] {
                new int[] { 1, 1, 1, 1, 0, 0, 0, 0, 1, 1 },
                new int[] { 1, 0, 0, 0, 1, 1, 0, 1, 1, 1 },
                new int[] { 0, 0, 1, 0, 1, 1, 1, 0, 0, 0 },
                new int[] { 1, 1, 1, 0, 0, 0, 1, 0, 0, 0 },
                new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
            };

            // Testing permanence values for spatialpooler bumpup weak columns method
            double[][] testingpermanences = new double[][] {
                new double[] { 0.200, 0.120, 0.090, 0.040, 0.000, 0.000, 0.000, 0.000,0.300,0.400 },
                new double[] { 0.150, 0.000, 0.000, 0.000, 0.180, 0.120, 0.000, 0.450,0.160,0.190 },
                new double[] { 0.000, 0.000, 0.014, 0.000, 0.032, 0.044, 0.110, 0.000,0.000,0.000 },
                new double[] { 0.041, 0.000, 0.000, 0.000, 0.000, 0.000, 0.178, 0.000,0.000,0.000 },
                new double[] { 0.100, 0.738, 0.045, 0.002, 0.050, 0.008, 0.208, 0.034,0.200,0.300 }
            };

            // Expected permanence values calculated manually using SynPermTrimThreshold and SynPermBelowStimulusInc when OverlapDutyCycles < MinOverlapDutyCycles
            // Expected permanence values are same as Testing permanence values in this case because of OverlapDutyCycles > MinOverlapDutyCycles
            double[][] ExpectedPermanences = new double[][] {
                new double[] { 0.200, 0.120, 0.090, 0.040, 0.000, 0.000, 0.000, 0.000,0.300,0.400 },
                new double[] { 0.150, 0.000, 0.000, 0.000, 0.180, 0.120, 0.000, 0.450,0.160,0.190 },
                new double[] { 0.000, 0.000, 0.014, 0.000, 0.032, 0.044, 0.110, 0.000,0.000,0.000 },
                new double[] { 0.041, 0.000, 0.000, 0.000, 0.000, 0.000, 0.178, 0.000,0.000,0.000 },
                new double[] { 0.100, 0.738, 0.045, 0.002, 0.050, 0.008, 0.208, 0.034,0.200,0.300 }
        };


            for (int i = 0; i < mem.HtmConfig.NumColumns; i++)
            {
                int[] indexes = ArrayUtils.IndexWhere(testingPools[i], n => n == 1);

                mem.GetColumn(i).SetProximalConnectedSynapsesForTest(mem, indexes);
                mem.GetColumn(i).SetPermanences(mem.HtmConfig, testingpermanences[i]);
            }

            //Execute method being tested
            sp.BumpUpWeakColumns(mem);

            for (int i = 0; i < mem.HtmConfig.NumColumns; i++)
            {
                // calculated permanence values from the spatialpooler BumpUpWeakColumns method
                double[] calculatedperms = mem.GetColumn(i).ProximalDendrite.RFPool.GetDensePermanences(mem.HtmConfig.NumInputs);
                for (int j = 0; j < ExpectedPermanences[i].Length; j++)
                {
                    // Veriying absolute values of manually calculated permanence and permanence values from BumpUpWeakColumns method
                    Assert.IsTrue(Math.Abs(ExpectedPermanences[i][j] - calculatedperms[j]) <= 0.01);
                }
            }
        }

    }
}