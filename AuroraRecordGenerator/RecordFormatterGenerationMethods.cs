﻿using Humanizer;
using System.Linq;
using System.Text;

namespace AuroraRecordGenerator
{
    internal partial class RecordFormatter
    {
        private void MakeCommonRecords()
        {
            var record = new StringBuilder();
            record.AppendLine("/// PUBLIC RECORD ///");
            record.AppendLine(MakeNameLine());
            record.AppendLine($"Date of Birth: {_targetRecord.BirthDate.ToString("MMMM")} {_targetRecord.BirthDate.Day.Ordinalize()}, {_targetRecord.BirthDate.Year}");
            record.AppendLine($"Species: {_targetRecord.Species.Humanize()}"); // might fuck up the names
            if (_targetRecord.Subspecies != SpeciesSubType.None)
            {
                record.AppendLine($"{_targetRecord.Subspecies.GetAttributeOfType<SubspeciesMetaAttribute>()?.FieldName ?? "Subspecies"}: {Utility.SubspeciesNiceName(_targetRecord.Subspecies)}");
            }
            if (_targetRecord.Pronouns.Any())
            {
               record.AppendLine($"Pronouns: {_targetRecord.Pronouns}");
            }
            if (_targetRecord.Citizenship.Any()) {
                record.AppendLine($"Citizenship: {_targetRecord.Citizenship}");
            }
            if (_targetRecord.NextOfKin.Any()) {
                record.AppendLine($"Next of Kin: {_targetRecord.NextOfKin}");
            }
            if (_targetRecord.EmployedAs.Any()) {
                record.AppendLine($"Employed As: {_targetRecord.EmployedAs}");
            }
            if (_targetRecord.CharHeight != null)
                record.AppendLine($"Height: {_targetRecord.CharHeight} cm ({Utility.CmToFeet(_targetRecord.CharHeight.Value)})");

            if (_targetRecord.Weight != null)
                record.AppendLine($"Weight: {_targetRecord.Weight} kg ({Utility.KgToLb(_targetRecord.Weight ?? 0)} lb)");

            // Eye color
            var trimmedEye = _targetRecord.EyeColor.Trim();
            record.AppendFormat("Eye Color: {0}\n", trimmedEye.Length > 0 ? trimmedEye : "Not specified.");

            var bodyColor = _targetRecord.SkinColor.Trim();
            record.AppendFormat("Skin/Body Color: {0}\n", bodyColor.Length > 0 ? bodyColor : "Not specified.");

            var hairColor = _targetRecord.HairColor.Trim();
            record.AppendFormat("Hair Color: {0}\n", hairColor.Length > 0 ? hairColor : "Not specified.");

            // identifying features
            var trimmedFeatures = _targetRecord.DistinguishingFeatures.Trim();
            record.Append("Distinguishing Features: ");
            record.AppendLine(trimmedFeatures.Length > 0 ? trimmedFeatures : "None noted.");

            record.AppendLine();

            // general notes
            WriteSectionIfAny(ref record,
                "Shared Employment Notes:",
                _employmentPublicRecord);

            WriteSectionIfAny(ref record,
                "Shared Medical Notes:",
                _medicalPublicRecord);

            WriteSectionIfAny(ref record,
                "Shared Security Notes:",
                _securityPublicRecord);

            _commonRecords = record.ToString();
        }

        private string MakeEmploymentRecords()
        {
            var recordText = new StringBuilder();
            if (_commonRecords.IsEmpty())
                MakeCommonRecords();

            recordText.Append(_commonRecords);

            if (!_employmentExperience.Any() &&
                !_employmentFormalEducation.Any() &&
                !_employmentPublicRecord.Any() &&
                !_employmentSkills.Any())
            {
                recordText.AppendLine("/// NO EMPLOYMENT RECORD FOUND ///");
            }
            else
            {
                recordText.AppendLine("/// EMPLOYMENT RECORD ///");
                recordText.AppendLine();

                WriteSectionIfAny(ref recordText,
                    "Employment History:",
                    _employmentExperience);

                WriteSectionIfAny(ref recordText,
                    "Qualifications:",
                    _employmentFormalEducation);

                WriteSectionIfAny(ref recordText,
                    "Other skills:",
                    _employmentSkills);
            }

            return recordText.ToString();
        }

        private string MakeMedicalRecords()
        {
            var recordText = new StringBuilder();
            if (_commonRecords.IsEmpty())
                MakeCommonRecords();

            recordText.Append(_commonRecords);

            // TODO: make this less horrible
            if (!_medicalHistory.Any() &&
                !_medicalNotes.Any() &&
                !_medicalPsychHistory.Any() &&
                !_medicalPsychNotes.Any() &&
                !_medicalPrescriptions.Any() &&
                !_targetRecord.NoBorg &&
                !_targetRecord.NoProsthetic &&
                !_targetRecord.NoRevive)
            {
                recordText.AppendLine("/// NO MEDICAL RECORD FOUND ///");
            }
            else
            {
                recordText.AppendLine("/// MEDICAL RECORD ///");
                recordText.AppendLine();

                recordText.AppendLine(
                    " The following information is protected by doctor-patient confidentiality laws. Do not release without patient's consent.\n");

                if (_targetRecord.NoBorg || _targetRecord.NoProsthetic || _targetRecord.NoRevive)
                {
                    recordText.AppendLine("IMPORTANT NOTES:");

                    if (_targetRecord.NoBorg)
                        MakeMedicalNote(ref recordText, "DO NOT BORGIFY");

                    if (_targetRecord.NoProsthetic)
                        MakeMedicalNote(ref recordText, "DO NOT INSTALL PROSTHETICS");

                    if (_targetRecord.NoRevive)
                        MakeMedicalNote(ref recordText, "DO NOT REVIVE");

                    recordText.AppendLine();
                }

                WriteSectionIfAny(ref recordText,
                    "Notes:",
                    _medicalNotes);

                WriteSectionIfAny(ref recordText,
                    "Medical History:",
                    _medicalHistory);

                WriteSectionIfAny(ref recordText,
                    "Psychiatric Notes:",
                    _medicalPsychNotes);

                WriteSectionIfAny(ref recordText,
                    "Psychiatric History:",
                    _medicalPsychHistory);

                WriteSectionIfAny(ref recordText,
                    "Prescriptions:",
                    _medicalPrescriptions);
            }

            return recordText.ToString();
        }

        private string MakeSecurityRecords()
        {
            var recordText = new StringBuilder();
            if (_commonRecords.IsEmpty())
                MakeCommonRecords();

            recordText.Append(_commonRecords);

            if (!_securityRecords.Any() && !_securityNotes.Any())
            {
                recordText.AppendLine("/// NO SECURITY RECORD FOUND ///");
            }
            else
            {
                recordText.AppendLine("/// SECURITY RECORD ///");
                recordText.AppendLine();

                WriteSectionIfAny(ref recordText,
                    "Notes:",
                    _securityNotes);

                WriteSectionIfAny(ref recordText,
                    "Record:",
                    _securityRecords);
            }

            return recordText.ToString();
        }
    }
}
