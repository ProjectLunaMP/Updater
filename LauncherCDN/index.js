const express = require("express");
const application = express();

application.listen(3000, () => {
  console.log("LauncherCDN on port " + 3000); // this isnt a actual CDN ;(
});

application.get("/files/:versionId/:filename", (req, res) => {
  const filePath = path.join(
    __dirname,
    `resources`,
    req.params.versionId,
    req.params.filename
  );
  fs.readFile(filePath, (err, data) => {
    if (err) {
      console.error("Not real:", err);
      return res.status(500).send("Error reading the file");
    }

    res.setHeader("Content-Type", "application/octet-stream");
    res.setHeader(
      "Content-Disposition",
      `attachment; filename="${req.params.filename}"`
    );
    res.send(data);
  });
});
