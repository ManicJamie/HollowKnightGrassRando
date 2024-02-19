import jsonc
import os, sys

script_directory = os.path.dirname(os.path.abspath(sys.argv[0]))

with open(f"{script_directory}/Locations.json") as f:
    raw: list[dict] = jsonc.load(f)

logicToLocations: dict[str, list[dict]] = {}

for loc in raw:
    if loc["logic"] not in logicToLocations:
        logicToLocations[loc["logic"]] = []
    logicToLocations[loc["logic"]].append(loc)

with open(f"{script_directory}/LogicGroupedLocations.json", "w") as f:
    jsonc.dump(logicToLocations, f, indent=4)