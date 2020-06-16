const Promise = require("bluebird");
const fetch = require("node-fetch");

fetch.Promise = Promise;

module.exports = blizzard;

function blizzard(connection) {
  this.db = connection;

  this.setup = function (token) {
    return this.setupRegion().then(this.setupRealm(token));
  };

  this.setupRegion = function () {
    return new Promise((resolve) => {
      const regionTable = this.db.prepare(
        "CREATE TABLE IF NOT EXISTS blizzardRegion ('id' INTEGER PRIMARY KEY, 'region' VARCHAR, 'name' VARCHAR, 'host' VARCHAR, 'locale' VARCHAR)"
      );
      regionTable.run();

      const regionConstraint = this.db.prepare(
        "CREATE UNIQUE INDEX IF NOT EXISTS idx_blizzardRegion_region ON blizzardRegion (region)"
      );
      regionConstraint.run();

      const regionInsert = this.db.prepare(
        "REPLACE INTO blizzardRegion(id, region, name, host, locale) VALUES (?, ?, ?, ?, ?)"
      );

      regionInsert.run(
        1,
        "EU",
        "Europe",
        "https://eu.api.blizzard.com",
        "en_GB"
      );
      regionInsert.run(
        2,
        "US",
        "America",
        "https://us.api.blizzard.com",
        "en_US"
      );
      regionInsert.run(
        3,
        "KR",
        "Korea",
        "https://kr.api.blizzard.com",
        "ko_KR"
      );
      regionInsert.run(
        4,
        "TW",
        "Taiwan",
        "https://tw.api.blizzard.com",
        "zh_TW"
      );

      resolve(console.log("\tBlizzard-Region: ✓"));
    });
  };

  this.setupRealm = function (token) {
    const realmTable = this.db.prepare(
      "CREATE TABLE IF NOT EXISTS blizzardRealm ('id' INTEGER PRIMARY KEY, 'regionId' INTEGER, 'name' VARCHAR, 'slug' VARCHAR)"
    );
    realmTable.run();

    const blizzardRegions = this.db
      .prepare("SELECT * FROM blizzardRegion")
      .all();

    var requests = [];

    const realmInsert = this.db.prepare(
      "REPLACE INTO blizzardRealm(id, regionId, name, slug) VALUES (?, ?, ?, ?)"
    );

    blizzardRegions.forEach((region) => {
      requests.push({
        region: region,
        url: `${region.host}/data/wow/realm/index?namespace=dynamic-${region.region}&locale=${region.locale}&access_token=${token}`,
      });
    });

    return Promise.all(
      requests.map((req) =>
        fetch(req.url)
          .then((res) => res.json())
          .then((res) => {
            res.realms.forEach((realm) => {
              realmInsert.run(realm.id, req.region.id, realm.name, realm.slug);
            });
          })
      )
    ).then(console.log("\tBlizzard-Realm: ✓"));
  };

  this.getRegion = function (regionName) {
    const sql = this.db
      .prepare("SELECT * FROM blizzardRegion WHERE LOWER(region) = LOWER(?)")
      .bind(regionName);
    return sql.get();
  };

  this.getRealm = function (regionId, realmSlug) {
    const sql = this.db
      .prepare(
        "SELECT * FROM blizzardRealm WHERE LOWER(slug) = LOWER(?) AND regionId = ?"
      )
      .bind(realmSlug, regionId);
    return sql.get();
  };
}
