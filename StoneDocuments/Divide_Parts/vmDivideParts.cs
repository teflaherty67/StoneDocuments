using Autodesk.Revit.DB;
using StoneDocuments.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace StoneDocuments
{
    public class vmDivideParts
    {
        private readonly Document curDoc;

        public ObservableCollection<string> SubcategoryNames { get; }

        public List<ElementId> SelectedWallIds { get; set; } = new List<ElementId>();

        public string Gap { get; set; }

        public string HorizontalType { get; set; }

        public string VerticalType { get; set; }

        internal vmDivideParts(Document doc)
        {
            curDoc = doc;
            SubcategoryNames = new ObservableCollection<string>(Utils.GetReferencePlaneSubcategoryNames(doc));
        }

        internal string Run()
        {
            if (SelectedWallIds == null || SelectedWallIds.Count == 0)
                return "No walls were selected.";

            if (!Utils.TryParseLength(curDoc, Gap, out double gapFeet))
                return "The gap value entered is not a valid length.";

            List<ReferencePlane> horizontalPlanes = Utils.GetReferencePlanesBySubcategory(curDoc, HorizontalType);
            List<ReferencePlane> verticalPlanes = Utils.GetReferencePlanesBySubcategory(curDoc, VerticalType);

            int wallsDivided = 0;
            int wallsSkipped = 0;

            using (Transaction t = new Transaction(curDoc, "Create & Divide Parts"))
            {
                t.Start();

                if (!PartUtils.AreElementsValidForCreateParts(curDoc, SelectedWallIds))
                {
                    t.RollBack();
                    return "The selected walls cannot be converted to parts.";
                }

                PartUtils.CreateParts(curDoc, SelectedWallIds);
                curDoc.Regenerate();

                foreach (ElementId wallId in SelectedWallIds)
                {
                    if (!(curDoc.GetElement(wallId) is Wall curWall))
                        continue;

                    List<ElementId> intersectingIds = horizontalPlanes
                        .Where(rp => Utils.DoesReferencePlaneIntersectWall(rp, curWall))
                        .Select(rp => rp.Id)
                        .Concat(verticalPlanes
                            .Where(rp => Utils.DoesReferencePlaneIntersectWall(rp, curWall))
                            .Select(rp => rp.Id))
                        .ToList();

                    if (intersectingIds.Count == 0)
                    {
                        wallsSkipped++;
                        continue;
                    }

                    ICollection<ElementId> hostParts = PartUtils.GetAssociatedParts(curDoc, wallId, false, false);

                    if (hostParts == null || hostParts.Count == 0 || !PartUtils.ArePartsValidForDivide(curDoc, hostParts))
                    {
                        wallsSkipped++;
                        continue;
                    }

                    PartUtils.DivideParts(curDoc, hostParts, intersectingIds, new List<Curve>(), null);
                    curDoc.Regenerate();

                    foreach (ElementId partId in PartUtils.GetAssociatedParts(curDoc, wallId, false, false))
                    {
                        Parameter gapParam = curDoc.GetElement(partId)?.LookupParameter("Gap");

                        if (gapParam != null && !gapParam.IsReadOnly)
                            gapParam.Set(gapFeet);
                    }

                    wallsDivided++;
                }

                t.Commit();
            }

            return $"Divided parts for {wallsDivided} wall(s). Skipped {wallsSkipped} wall(s) with no matching reference planes.";
        }
    }
}
