import jsonc
import os, sys

script_directory = os.path.dirname(os.path.abspath(sys.argv[0]))

with open(f"{script_directory}/Locations_raw.json") as f:
    raw: list[dict] = jsonc.load(f)

with open(f"{script_directory}/TranslatorDict.json") as f:
    translate: dict = jsonc.load(f)

areaIds = {"King's": 1,
           "Dirtmouth": 2,
           "Crossroads": 4,
           "Greenpath": 8,
           "Fungal": 16,
           "Fog": 32,
           "Gardens": 64,
           "Grounds": 128,
           "City": 256,
           "Edge": 512,
           "Deepnest": 1024,
           "Basin": 2048,
           "Abyss": 4096,
           "Palace": 8192,
           "Godhome": 8192 * 2}

ghostRefs = {"Dream_01_False_Knight": "Crossroads",
             "Dream_02_Mage_Lord": "City",
             "Dream_Backer_Shrine": "Grounds",
             "Dream_Nailcollection": "Grounds"}

def getPool(sceneName):
    areaName = getAreaName(sceneName)
    if areaName == "Dream": areaName = ghostRefs[sceneName]
    return areaIds[areaName]

godhomes = ["GG_Broken_Vessel",
             "GG_Ghost_Marmu",
               "GG_Ghost_Marmu_V",
                 "GG_Hornet_2",
                 "GG_Lost_Kin",
                 "GG_Mega_Moss_Charger",
                 "GG_Traitor_Lord"]
def getAreaName(sceneName):
    if sceneName in godhomes: return "Godhome"
    return translate[sceneName].split("_")[0]

def getSceneTranslation(sceneName):
    return translate.get(sceneName, sceneName)

def getLocationName(sceneName, id):
    return f"Grass-{getSceneTranslation(sceneName)}-{id}"

output = []
scene_ids = {}
for grass_raw in raw:
    grass = {}
    location = grass_raw["locations"][0]
    sceneName = grass_raw["sceneName"]
    if sceneName in scene_ids:
        id = scene_ids[sceneName]
        scene_ids[sceneName] += 1
    else:
        id = 0
        scene_ids[sceneName] = 1
    grass["locationName"] = getLocationName(sceneName, id)
    grass["id"] = id
    grass["spriteTarget"] = 0 # enum for which sprite to use
    grass["grassArea"] = getPool(sceneName)
    grass["areaType"] = int(sceneName in ghostRefs)
    grass["logic"] = grass_raw["logic"]
    grass.update({"sceneName": sceneName, "objectName": grass_raw["gameObj"],
            "x": location["x"], "y": location["y"]})
    output.append(grass)

print(len(output))

with open(f"{script_directory}/Locations.json", "w") as f:
    jsonc.dump(output, f, indent=4)
