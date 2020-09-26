﻿using ColossalFramework;
using RealConstruction.Util;
using UnityEngine;

namespace RealConstruction.NewAI
{
    public class ResourceBuildingAI
    {
        public static bool IsSpecialBuilding(ushort id)
        {
            //0 normal building
            //1-3 resource building
            //4 normal building and do not need resource
            if (MainDataStore.resourceCategory[id] != 0 && MainDataStore.resourceCategory[id] != 4)
            {
                return true;
            }
            return false;
        }

        public static void ProcessCityResourceDepartmentBuildingGoods(ushort buildingID, ref Building buildingData)
        {
            int reduceCargoDiv = 1;
            float averageSalary = 40f;
            if (Loader.isRealCityRunning)
            {
                RealCityUtil.InitDelegate();
                reduceCargoDiv = RealCityUtil.GetReduceCargoDiv();
                averageSalary = RealCityUtil.GetAverageSalary();
            }
            int materialConsumption = (int)(1f * averageSalary / reduceCargoDiv);
            if (materialConsumption == 0)
                materialConsumption = 1;
            if (buildingData.m_fireIntensity == 0 && buildingData.m_flags.IsFlagSet(Building.Flags.Completed))
            {
                if (MainDataStore.lumberBuffer[buildingID] > materialConsumption && MainDataStore.coalBuffer[buildingID] > materialConsumption && MainDataStore.constructionResourceBuffer[buildingID] < 64000)
                {
                    if (MainDataStore.resourceCategory[buildingID] == 1 || MainDataStore.resourceCategory[buildingID] == 2)
                    {
                        MainDataStore.lumberBuffer[buildingID] -= (ushort)materialConsumption;
                        MainDataStore.coalBuffer[buildingID] -= (ushort)materialConsumption;
                        MainDataStore.constructionResourceBuffer[buildingID] += 400;
                    }
                }

                if (MainDataStore.petrolBuffer[buildingID] > materialConsumption && MainDataStore.foodBuffer[buildingID] > materialConsumption && MainDataStore.operationResourceBuffer[buildingID] < 64000)
                {
                    if (MainDataStore.resourceCategory[buildingID] == 1 || MainDataStore.resourceCategory[buildingID] == 3)
                    {
                        MainDataStore.petrolBuffer[buildingID] -= (ushort)materialConsumption;
                        MainDataStore.foodBuffer[buildingID] -= (ushort)materialConsumption;
                        MainDataStore.operationResourceBuffer[buildingID] += 400;
                    }
                }
            }
        }

        public static void ProcessCityResourceDepartmentBuildingOutgoing(ushort buildingID, ref Building buildingData)
        {
            int num27 = 0;
            int num28 = 0;
            int num29 = 0;
            int value = 0;
            TransferManager.TransferReason outgoingTransferReason = default(TransferManager.TransferReason);

            //constructionResource
            System.Random rand = new System.Random();
            outgoingTransferReason = (TransferManager.TransferReason)124;
            if (outgoingTransferReason != TransferManager.TransferReason.None)
            {
                CaculationVehicle.CustomCalculateOwnVehicles(buildingID, ref buildingData, outgoingTransferReason, ref num27, ref num28, ref num29, ref value);
                buildingData.m_tempExport = (byte)Mathf.Clamp(value, (int)buildingData.m_tempExport, 255);
            }

            if (buildingData.m_fireIntensity == 0 && outgoingTransferReason != TransferManager.TransferReason.None && buildingData.m_flags.IsFlagSet(Building.Flags.Completed))
            {
                int num36 = 20;
                int customBuffer = MainDataStore.constructionResourceBuffer[buildingID];
                if (customBuffer >= 8000 && num27 < num36)
                {
                    TransferManager.TransferOffer offer2 = default(TransferManager.TransferOffer);
                    offer2.Priority = rand.Next(8);
                    offer2.Building = buildingID;
                    offer2.Position = buildingData.m_position;
                    offer2.Amount = Mathf.Min(customBuffer / 8000, num36 - num27);
                    offer2.Active = true;
                    Singleton<TransferManager>.instance.AddOutgoingOffer(outgoingTransferReason, offer2);
                }
            }

            //operationResource
            outgoingTransferReason = (TransferManager.TransferReason)125;
            if (outgoingTransferReason != TransferManager.TransferReason.None)
            {
                CaculationVehicle.CustomCalculateOwnVehicles(buildingID, ref buildingData, outgoingTransferReason, ref num27, ref num28, ref num29, ref value);
                buildingData.m_tempExport = (byte)Mathf.Clamp(value, (int)buildingData.m_tempExport, 255);
            }

            if (buildingData.m_fireIntensity == 0 && outgoingTransferReason != TransferManager.TransferReason.None && buildingData.m_flags.IsFlagSet(Building.Flags.Completed))
            {
                int num36 = 20;
                int customBuffer = MainDataStore.operationResourceBuffer[buildingID];
                if (customBuffer >= 8000 && num27 < num36)
                {
                    TransferManager.TransferOffer offer2 = default(TransferManager.TransferOffer);
                    offer2.Priority = rand.Next(8);
                    offer2.Building = buildingID;
                    offer2.Position = buildingData.m_position;
                    offer2.Amount = Mathf.Min((customBuffer) / 8000, num36 - num27);
                    offer2.Active = true;
                    Singleton<TransferManager>.instance.AddOutgoingOffer(outgoingTransferReason, offer2);
                }
            }
        }

        public static void ProcessCityResourceDepartmentBuildingIncoming(ushort buildingID, ref Building buildingData)
        {
            int num27 = 0;
            int num28 = 0;
            int num29 = 0;
            int value = 0;
            int num34 = 0;
            TransferManager.TransferReason incomingTransferReason = default(TransferManager.TransferReason);
            //Foods
            if (MainDataStore.resourceCategory[buildingID] == 1 || MainDataStore.resourceCategory[buildingID] == 3)
            {
                incomingTransferReason = TransferManager.TransferReason.Food;
                if (incomingTransferReason != TransferManager.TransferReason.None)
                {
                    CaculationVehicle.CustomCalculateGuestVehicles(buildingID, ref buildingData, incomingTransferReason, ref num27, ref num28, ref num29, ref value);
                    buildingData.m_tempImport = (byte)Mathf.Clamp(value, (int)buildingData.m_tempImport, 255);
                }

                num34 = 4000 - MainDataStore.foodBuffer[buildingID] - num29;
                if (buildingData.m_fireIntensity == 0 && buildingData.m_flags.IsFlagSet(Building.Flags.Completed))
                {
                    if (num34 >= 0)
                    {
                        TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                        offer.Priority = 7;
                        offer.Building = buildingID;
                        offer.Position = buildingData.m_position;
                        offer.Amount = 1;
                        offer.Active = false;
                        Singleton<TransferManager>.instance.AddIncomingOffer(incomingTransferReason, offer);
                    }
                }
                //Petrol
                incomingTransferReason = TransferManager.TransferReason.Petrol;
                num27 = 0;
                num28 = 0;
                num29 = 0;
                value = 0;
                num34 = 0;
                if (incomingTransferReason != TransferManager.TransferReason.None && buildingData.m_flags.IsFlagSet(Building.Flags.Completed))
                {
                    CaculationVehicle.CustomCalculateGuestVehicles(buildingID, ref buildingData, incomingTransferReason, ref num27, ref num28, ref num29, ref value);
                    buildingData.m_tempImport = (byte)Mathf.Clamp(value, (int)buildingData.m_tempImport, 255);
                }

                num34 = 4000 - MainDataStore.petrolBuffer[buildingID] - num29;
                if (buildingData.m_fireIntensity == 0)
                {
                    if (num34 >= 0)
                    {
                        TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                        offer.Priority = 7;
                        offer.Building = buildingID;
                        offer.Position = buildingData.m_position;
                        offer.Amount = 1;
                        offer.Active = false;
                        Singleton<TransferManager>.instance.AddIncomingOffer(incomingTransferReason, offer);
                    }
                }
            }
            //Coal
            if (MainDataStore.resourceCategory[buildingID] == 1 || MainDataStore.resourceCategory[buildingID] == 2)
            {
                incomingTransferReason = TransferManager.TransferReason.Coal;
                num27 = 0;
                num28 = 0;
                num29 = 0;
                value = 0;
                num34 = 0;
                if (incomingTransferReason != TransferManager.TransferReason.None && buildingData.m_flags.IsFlagSet(Building.Flags.Completed))
                {
                    CaculationVehicle.CustomCalculateGuestVehicles(buildingID, ref buildingData, incomingTransferReason, ref num27, ref num28, ref num29, ref value);
                    buildingData.m_tempImport = (byte)Mathf.Clamp(value, (int)buildingData.m_tempImport, 255);
                }

                num34 = 4000 - MainDataStore.coalBuffer[buildingID] - num29;
                if (buildingData.m_fireIntensity == 0)
                {
                    if (num34 >= 0)
                    {
                        TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                        offer.Priority = 7;
                        offer.Building = buildingID;
                        offer.Position = buildingData.m_position;
                        offer.Amount = 1;
                        offer.Active = false;
                        Singleton<TransferManager>.instance.AddIncomingOffer(incomingTransferReason, offer);
                    }
                }

                //Lumber
                incomingTransferReason = TransferManager.TransferReason.Lumber;
                num27 = 0;
                num28 = 0;
                num29 = 0;
                value = 0;
                num34 = 0;
                if (incomingTransferReason != TransferManager.TransferReason.None)
                {
                    CaculationVehicle.CustomCalculateGuestVehicles(buildingID, ref buildingData, incomingTransferReason, ref num27, ref num28, ref num29, ref value);
                    buildingData.m_tempImport = (byte)Mathf.Clamp(value, (int)buildingData.m_tempImport, 255);
                }

                num34 = 4000 - MainDataStore.lumberBuffer[buildingID] - num29;
                if (buildingData.m_fireIntensity == 0 && buildingData.m_flags.IsFlagSet(Building.Flags.Completed))
                {
                    if (num34 >= 0)
                    {
                        TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                        offer.Priority = 7;
                        offer.Building = buildingID;
                        offer.Position = buildingData.m_position;
                        offer.Amount = 1;
                        offer.Active = false;
                        Singleton<TransferManager>.instance.AddIncomingOffer(incomingTransferReason, offer);
                    }
                }
            }
        }
    }
}
