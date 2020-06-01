module.exports = blizzard;

function blizzard(connection) {
  this.db = connection;

  this.setup = function () {
    this.setupRegion();
    this.setupRealm();
  };

  this.setupRegion = function () {
    const regionTable = this.db.prepare(
      "CREATE TABLE IF NOT EXISTS blizzardRegion ('id' INTEGER PRIMARY KEY AUTOINCREMENT, 'region' VARCHAR, 'name' VARCHAR, 'host' VARCHAR, 'locale' VARCHAR)"
    );
    regionTable.run();

    const regionConstraint = this.db.prepare("CREATE UNIQUE INDEX IF NOT EXISTS idx_blizzardRegion_region ON blizzardRegion (region)");
    regionConstraint.run();

    const regionInsert = this.db.prepare(
      "REPLACE INTO blizzardRegion(region, name, host, locale) VALUES (?, ?, ?, ?)"
    );
    regionInsert.run(
      "US",
      "North America",
      "https://us.api.blizzard.com/",
      "en_US"
    );
    regionInsert.run("EU", "Europe", "https://eu.api.blizzard.com/", "en_GB");
    regionInsert.run("KR", "Korea", "https://kr.api.blizzard.com/", "ko_KR");
    regionInsert.run("TW", "Taiwan", "https://tw.api.blizzard.com/", "zh_TW");
    regionInsert.run(
      "CN",
      "China",
      "https://gateway.battlenet.com.cn/",
      "zh_CN"
    );

    console.log("\tBlizzard-Region: ✓");
  };

  this.setupRealm = function () {
    const realmTable = this.db.prepare(
      "CREATE TABLE IF NOT EXISTS blizzardRealm ('id' INTEGER PRIMARY KEY, 'regionId' INTEGER, 'name' VARCHAR, 'slug' VARCHAR)"
    );
    realmTable.run();

    const realmInsert = this.db.prepare(
      "REPLACE INTO blizzardRealm(id, regionId, name, slug) VALUES (?, ?, ?, ?)"
    );

    // realmInsert.run("EU", "Europe", "https://eu.api.blizzard.com/", "en_GB");
    

    const stmt = this.db.prepare("SELECT * FROM blizzardRealm").all();
    console.log(stmt);

    console.log("\tBlizzard-Realm: ✓");
  };
}
