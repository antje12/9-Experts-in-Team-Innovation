#!/bin/bash
/opt/hive/bin/schematool -initSchema -dbType postgres
/opt/hive/bin/hive --service metastore