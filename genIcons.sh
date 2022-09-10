#!/bin/bash
ats="witchingandwizardry"
tpath="$(pwd)/resources/assets/$ats/textures/blocks/misc/chalk"
ipath="$(pwd)/resources/assets/$ats/textures/icons/chalk"


for entry in "$tpath"/*
do
  readarray -d - -t str <<<"$(basename -z $entry .png)"
  png2svg $entry >"$ipath/${str[1]}-$(echo ${str[2]} | tr -d '\n')".svg
done
clear