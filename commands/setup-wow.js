module.exports = {
  name: "setup-wow",
  description:
    "Setup the server to use a default World of Warcraft guild for WoW queries.",
  guildOnly: true,
  adminOnly: true,
  args: true,
  usage: "[region] [realm] [guild-name]",
  async execute(message, db_context, args) {
    if (args.length !== 3) {
      return message.reply("Incorrect amount of arguments recieved.");
    }

    const region = args[0];
    if (!validateRegion(region)) {
      return message.reply("region not valid.");
    }

    const realm = args[1];
    if (!validateRealm(locale, realm)) {
      return message.reply("realm not valid.");
    }

    const guildName = args[2];

    db_context.guildwow.action(message.guild.id, region, realm, guildName);
  },
};

function validateRegion(region) {
  return true;
}

function validateRealm(region, realm) {
  return true;
}
