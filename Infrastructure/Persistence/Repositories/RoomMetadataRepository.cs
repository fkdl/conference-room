﻿using MongoDB.Driver;
using MongoDB.Driver.Builders;
using RightpointLabs.ConferenceRoom.Domain.Repositories;
using RightpointLabs.ConferenceRoom.Infrastructure.Persistence.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using RightpointLabs.ConferenceRoom.Domain.Models.Entities;

namespace RightpointLabs.ConferenceRoom.Infrastructure.Persistence.Repositories
{
    public class RoomMetadataRepository : EntityRepository<RoomMetadataEntity>, IRoomMetadataRepository
    {
        public RoomMetadataRepository(RoomMetadataEntityCollectionDefinition collectionDefinition)
            : base(collectionDefinition)
        {
        }

        public RoomMetadataEntity GetRoomInfo(string roomId)
        {
            var q = Query<RoomMetadataEntity>.Where(i => i.Id == roomId);
            return this.Collection.FindOne(q);
        }

        public IEnumerable<RoomMetadataEntity> GetRoomInfosForBuilding(string buildingId)
        {
            var q = Query<RoomMetadataEntity>.Where(i => i.BuildingId == buildingId);
            return this.Collection.Find(q);
        }

        public IEnumerable<RoomMetadataEntity> GetRoomInfosForOrganization(string organizationId)
        {
            var q = Query<RoomMetadataEntity>.Where(i => i.OrganizationId == organizationId);
            return this.Collection.Find(q);
        }
    }
}