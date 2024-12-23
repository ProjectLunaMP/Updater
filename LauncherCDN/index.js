const express = require("express");
const application = express();
const fs = require("fs");
const path = require("path");

application.listen(3000, () => {
  console.log("LauncherCDN on port " + 3000); // this isnt a actual CDN ;(
});

application.get("/files/:images", (req, res) => {
  try {
    const filePath = path.resolve(
      __dirname,
      "resources",
      "images",
      req.params.images
    );

    fs.stat(filePath, (err, stat) => {
      if (err || !stat.isFile()) {
        return res.status(404).send("File not found");
      }

      const ext = path.extname(filePath).toLowerCase();
      let contentType = "application/octet-stream";

      if (ext === ".bmp") {
        contentType = "image/bmp";
      }

      res.setHeader("Content-Type", contentType);
      res.setHeader("Cache-Control", "public, max-age=3600");
      fs.createReadStream(filePath).pipe(res);
    });
  } catch (err) {
    
  }
});

application.get("/files/:versionId/:filename", (req, res) => {
  const filePath = path.resolve(
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

application.use("/*", (req, res) => res.send());
