const { series } = require('gulp');
let { clean, restore, build, test, pack, publish, run } = require('gulp-dotnet-cli');
let gulp = require('gulp');
var exec = require('child_process').exec;
var msbuild = require("gulp-msbuild");
gulpConfig = require('./gulpConfig');
const fs = require('fs')
const path = './TransactionLimitDB/bin/Release/TransactionLimitDB.dacpac'


async function dotnetBuild() {
  exec("Powershell.exe Remove-Item .\\TransactionLimitDB\\bin\\Release\\TransactionLimitDB.dacpac")
  gulp.src(gulpConfig.paths.core, { read: false })
    .pipe(build());
  console.log("Core Build Tamamlandı.")
}

async function sqlbuild(err, stdout, stderr) {
  gulp.src(".\\TransactionLimitDB\\TransactionLimitDB.sqlproj")
    .pipe(msbuild({
      stderr: true,
      stdout: true,
      logCommand: false,
      toolsVersion: 'auto'
    }));
  console.log(err, stdout, stderr);
}

async function sqlpublish() {
  var control = false;
  var counter = 0;
  var controller = setInterval(function () {
    counter++;
    fs.access(path, fs.F_OK, (err) => {
      if (err) control = false;
      else return control = true;
    })
    if (control == true) {
      exec('sqlpackage /Action:Publish  /SourceFile:".\\TransactionLimitDB\\bin\\Release\\TransactionLimitDB.dacpac" /TargetConnectionString:"Server=.\\SQLEXPRESS; Database=TransactionLimitDB; Trusted_Connection=True; MultipleActiveResultSets=true"', function (err, stdout, stderr) {
        console.log(err, stdout, stderr)
        console.log("DB Publish Tamamlandı.")
      });
      clearInterval(controller);
    }
    try {
      if (counter > 15) throw "HATA! Dacpac dosyasi bulunamadi!";
    }
    catch (err) {
      console.log(err);
      clearInterval(controller);
    }

  }, 1000);
}

exports.build = series(dotnetBuild, sqlbuild, sqlpublish);



