namespace DA_Assets.FCU
{
    public static class FcuLocExtensions
    {
        public static string Localize(this FcuLocKey key, params object[] args) =>
            FcuConfig.Instance.Localizator.GetLocalizedText(key, args);
    }

    public enum FcuLocKey
    {
        // Logs
        log_added_total,
        log_api_waiting,
        log_auth_complete,
        log_cant_auth,
        log_cant_draw_object,
        log_cant_find_package,
        log_cant_get_images,
        log_cant_get_part_of_frames,
        log_cant_get_image_links,
        log_cant_execute_because_no_backup,
        log_component_not_selected_in_hierarchy,
        log_current_canvas_metas_destroy,
        log_dev_function_enabled,
        log_downloading_fonts,
        log_downloading_images,
        log_drawn_count,
        log_enable_http_project_settings,
        log_fcu_assigned,
        log_generating_sprites,
        log_generating_tmp_fonts,
        log_getting_frames,
        log_getting_links,
        log_import_complete,
        log_import_task_canceled,
        log_incorrect_selection,
        log_instantiate_game_objects,
        log_links_added,
        log_local_prefabs_found,
        log_malformed_url,
        log_mark_as_sprite,
        log_need_auth,
        log_no_google_fonts_api_key,
        log_no_sync_helper,
        log_nothing_to_import,
        log_open_auth_page,
        log_prefabs_created,
        log_project_downloaded,
        log_project_empty,
        log_project_not_found,
        log_search_local_prefabs,
        log_set_anchors,
        log_ssl_error,
        log_start_adding_to_fonts_list,
        log_start_creating_prefabs,
        log_start_download_images,
        log_start_setting_transform,
        log_tagging,
        log_unknown_aligment,
        log_unknown_error,
        log_not_authorized,
        log_feature_not_available_with,
        log_incorrent_project_url,
        log_name_linking_not_recommended,
        log_svg_scale_1,
        log_import_failed_incompatible,
        log_import_failed_enable_required,
        log_import_failed_unsupported,
        log_tagging_by_parts,
        log_asset_not_imported,

        //unknown

        loading_google_fonts,
        cant_download_fonts,
        cant_generate_fonts,
        cant_load_sprites,
        cant_download_sprite,

        // Labels and Tooltips
        label_advanced_mode,
        tooltip_advanced_mode,

        label_add_tmp_fonts_from_folder,
        tooltip_add_fonts_from_folder,

        label_add_ttf_fonts_from_folder,
        tooltip_add_ttf_fonts_from_folder,

        label_all,
        tooltip_all,

        label_apply_and_continue,
        tooltip_apply_and_continue,

        label_asset,
        tooltip_asset,

        label_asset_creator_settings,
        tooltip_asset_creator_settings,

        label_asset_dependencies,
        tooltip_asset_dependencies,

        label_atlas_padding,
        tooltip_atlas_padding,

        label_atlas_population_mode,
        tooltip_atlas_population_mode,

        label_atlas_resolution,
        tooltip_atlas_resolution,

        label_auto_disable_compress_assets_on_import,
        tooltip_auto_disable_compress_assets_on_import,

        label_auto_size,
        tooltip_auto_size,

        label_best_fit,
        tooltip_best_fit,

        label_beta_version,
        tooltip_beta_version,

        label_buggy_version,
        tooltip_buggy_version,

        label_button_settings,
        tooltip_button_settings,

        label_button_type,
        tooltip_button_type,

        label_change,
        tooltip_change,

        label_changed_in_figma,
        tooltip_changed_in_figma,

        label_changed_in_unity,
        tooltip_changed_in_unity,

        label_comparer_desc,
        tooltip_comparer_desc,

        label_components_settings,
        tooltip_import_components,

        label_compression_quality,
        tooltip_compression_quality,

        label_copy_new_data,
        tooltip_copy_new_data,

        label_copy_old_data,
        tooltip_copy_old_data,

        label_copy_to_clipboard,
        tooltip_copy_to_clipboard,

        label_crunched_compression,
        tooltip_crunched_compression,

        label_csv_separator,
        tooltip_csv_separator,

        label_custom_pivot,
        tooltip_custom_pivot,

        label_dabutton_settings,
        tooltip_dabutton_settings,

        label_debug_mode,
        tooltip_debug_mode,

        label_debug_tools,
        label_debug,
        tooltip_debug_tools,

        label_dependencies,
        tooltip_dependencies,

        label_disable_compress_assets_on_import,
        tooltip_disable_compress_assets_on_import,

        label_disabled_color,
        tooltip_disabled_color,

        label_dont_remove_fcu_meta,
        tooltip_dont_remove_fcu_meta,

        label_download_fonts_from_project,
        tooltip_download_fonts_from_project,

        label_sprite_download_options,
        tooltip_sprite_download_options,

        label_download_unsupported_gradients,
        tooltip_download_unsupported_gradients,

        label_fade_duration,
        tooltip_fade_duration,

        label_figma_auth,
        tooltip_figma_auth,

        label_figma_color,
        tooltip_figma_color,

        label_figma_comp,
        tooltip_figma_comp,

        label_figma_comp_desc,
        tooltip_figma_comp_desc,

        label_figma_layout_culture,
        tooltip_figma_layout_culture,

        label_find_added_objects,
        tooltip_find_added_objects,

        label_flip_x,
        tooltip_flip_x,

        label_flip_y,
        tooltip_flip_y,

        label_font_settings,
        tooltip_font_settings,

        label_font_subset,
        tooltip_font_subset,

        label_force_fix,
        tooltip_force_fix,

        label_frames_to_import,
        tooltip_frames_to_import,

        label_generate_physics_shape,
        tooltip_generate_physics_shape,

        label_gradient_resolution,
        tooltip_gradient_resolution,

        label_highlighted_color,
        tooltip_highlighted_color,

        label_horizontal_mapping,
        tooltip_horizontal_mapping,

        label_horizontal_overflow,
        tooltip_horizontal_overflow,

        label_https_setting,
        tooltip_https_setting,

        label_image_and_sprites,
        tooltip_image_and_sprites,

        label_image_component,
        tooltip_image_component,

        label_image_type,
        tooltip_image_type,

        label_import,
        tooltip_import,

        label_import_events,
        tooltip_import_events,

        log_import_stoped_because_error,
        tooltip_import_stoped_because_error,

        log_import_stoped_manually,
        tooltip_stop_import,

        label_kerning,
        tooltip_kerning,

        label_kilobytes,
        tooltip_kilobytes,

        label_layout_culture,
        tooltip_layout_culture,

        label_line_spacing,
        tooltip_line_spacing,

        label_loc_case_type,
        tooltip_loc_case_type,

        label_loc_component,
        tooltip_loc_component,

        label_loc_file_name,
        tooltip_loc_file_name,

        label_loc_file_path,
        tooltip_loc_file_path,

        label_localization_settings,
        tooltip_localization_settings,

        label_localizator,
        tooltip_localizator,

        label_log_default,
        tooltip_log_default,

        label_log_downloadable,
        tooltip_log_downloadable,

        label_log_go_drawer,
        tooltip_log_go_drawer,

        label_log_set_tag,
        tooltip_log_set_tag,

        label_log_transform,
        tooltip_log_transform,

        label_made_by,
        tooltip_made_by,

        label_main_settings,
        tooltip_main_settings,

        label_mask_interaction,
        tooltip_mask_interaction,

        label_maskable,
        tooltip_maskable,

        label_max_cord_deviation_enabled,
        tooltip_max_cord_deviation_enabled,

        label_max_tangent_angle_enabled,
        tooltip_max_tangent_angle_enabled,

        label_mipmap_enabled,
        tooltip_mipmap_enabled,

        label_missings_in_frame,
        tooltip_missings_in_frame,

        label_namespace,
        tooltip_namespace,

        label_new,
        tooltip_new,

        label_no_recent_projects,
        tooltip_no_recent_projects,

        label_normal_color,
        tooltip_normal_color,

        label_old_data,
        tooltip_old_data,

        label_override_line_spacing_px,
        tooltip_override_line_spacing_px,

        label_override_tags,
        tooltip_override_tags,

        label_parse_escape_characters,
        tooltip_parse_escape_characters,

        label_pixels_per_unit,
        tooltip_pixels_per_unit,

        label_positioning_mode,
        tooltip_positioning_mode,

        label_prefab_creator,
        tooltip_prefab_creator,

        label_prefabs_path,
        tooltip_prefabs_path,

        label_preserve_aspect,
        tooltip_preserve_aspect,

        label_preserve_numbers,
        tooltip_preserve_numbers,

        label_pressed_color,
        tooltip_pressed_color,

        label_project_url,
        tooltip_project_url,

        label_pui_falloff_distance,
        tooltip_pui_falloff_distance,

        label_rate,
        tooltip_rate,

        label_rateme,
        tooltip_rateme,

        label_rateme_desc,
        tooltip_rateme_desc,

        label_raycast_padding,
        tooltip_raycast_padding,

        label_raycast_target,
        tooltip_raycast_target,

        label_redownload_sprites,
        tooltip_redownload_sprites,

        label_remove,
        tooltip_remove,

        label_remove_unused_sprites,
        tooltip_remove_unused_sprites,

        label_render_mode,
        tooltip_render_mode,

        label_rich_text,
        tooltip_rich_text,

        label_rtl_textmeshpro_settings,
        tooltip_rtl_textmeshpro_settings,

        label_sampling_steps,
        tooltip_sampling_steps,

        label_selected_color,
        tooltip_selected_color,

        label_shadow_type,
        tooltip_shadow_type,

        label_shadows_tab,
        tooltip_shadows_tab,

        label_sort_point,
        tooltip_sort_point,

        label_sorting_layer,
        tooltip_sorting_layer,

        label_sprite_import_mode,
        tooltip_sprite_import_mode,

        label_step_distance,
        tooltip_step_distance,

        label_svg_image_settings,
        tooltip_svg_image_settings,

        label_svg_importer_settings,
        tooltip_svg_importer_settings,

        label_svg_type,
        tooltip_svg_type,

        label_text_and_fonts,
        tooltip_text_and_fonts,

        label_text_and_font_settings,
        tooltip_text_and_font_settings,

        label_text_component,
        tooltip_text_component,

        label_textmeshpro_settings,
        tooltip_textmeshpro_settings,

        label_texture_compression,
        tooltip_texture_compression,

        label_texture_importer_settings,
        tooltip_texture_importer_settings,

        label_texture_type,
        tooltip_texture_type,

        label_ttf_path,
        tooltip_ttf_path,

        label_ui_framework,
        tooltip_ui_framework,

        label_ui_toolkit,
        tooltip_ui_toolkit,

        label_unity_comp,
        tooltip_unity_comp,

        label_unity_image_settings,
        tooltip_unity_image_settings,

        label_unity_text_settings,
        tooltip_unity_text_settings,

        label_uitk_linking_mode,
        tooltip_uitk_linking_mode,

        label_uitk_output_path,
        tooltip_uitk_output_path,

        label_use_i2localization,
        tooltip_use_i2localization,

        label_visible_descender,
        tooltip_visible_descender,

        label_viewport_options,
        tooltip_viewport_options,

        label_vertical_mapping,
        tooltip_vertical_mapping,

        label_vertical_overflow,
        tooltip_vertical_overflow,

        label_without_changes,
        tooltip_label_without_changes,

        label_wrapping,
        tooltip_wrapping,

        label_xml_parsing,
        tooltip_xml_parsing,

        label_procedural_ui_settings,
        tooltip_procedural_ui_settings,

        label_pui_settings,
        tooltip_pui_settings,

        label_color_multiplier,
        tooltip_color_multiplier,

        label_mpuikit_settings,
        tooltip_mpuikit_settings,

        label_sr_settings,
        tooltip_sr_settings,

        label_is_readable,
        tooltip_is_readable,

        label_next_order_step,
        tooltip_next_order_step,

        label_raw_import,
        tooltip_raw_import,
       
        label_images_and_sprites_tab,
        tooltip_images_and_sprites_tab,

        label_buttons_tab,
        tooltip_buttons_tab,

        label_ui_toolkit_tab,
        tooltip_ui_toolkit_tab,

        label_nova_components,
        tooltip_nova_components,

        label_fcu,
        tooltip_fcu,

        label_settings,
        tooltip_settings,
       
        label_script_generator,
        tooltip_script_generator,

        label_enabled,
        tooltip_enabled,

        label_scripts_output_path,
        tooltip_scripts_output_path,

        label_select_folder,
        tooltip_select_folder,

        label_text_prefab_naming_mode,
        tooltip_text_prefab_naming_mode,

        label_humanized_color,
        tooltip_humanized_color,

        label_hex_color,
        tooltip_hex_color,

        label_log_component_drawer,
        tooltip_log_component_drawer,

        label_log_hash_generator_drawer,
        tooltip_log_hash_generator_drawer,

        label_override_tmp_letter_spacing,
        tooltip_override_tmp_letter_spacing,

        label_object_comparer,
        tooltip_object_comparer,

        label_fcu_is_null,
        tooltip_fcu_is_null,

        label_more_about_layout_updating,
        tooltip_more_about_layout_updating,

        tooltip_open_fcu_window,
        tooltip_change_window_mode,

        label_orthographic_mode,
        tooltip_orthographic_mode,

        label_extra_padding,
        tooltip_extra_padding,

        label_overflow,
        tooltip_overflow,

        label_geometry_sorting,
        tooltip_geometry_sorting,

        label_shader,
        tooltip_shader,

        label_farsi,
        tooltip_farsi,

        label_fix_tags,
        tooltip_fix_tags,

        label_user_name,
        tooltip_user_id,

        label_asset_instance_id,
        tooltip_asset_instance_id,

        label_stable_version,
        tooltip_stable_version,

        label_open_diff_checker,
        tooltip_open_diff_checker,

        label_go_name_max_length,
        tooltip_go_name_max_length,

        label_text_name_max_length,
        tooltip_text_name_max_length,

        label_go_layer,
        tooltip_go_layer,

        label_pivot_type,
        tooltip_pivot_type,

        label_components_to_import,
        tooltip_components_to_import,

        label_components_with_count,
        tooltip_components_with_count,

        tooltip_recent_projects,
        tooltip_download_project,

        label_sampling_point_size,
        tooltip_sampling_point_size,

        label_enable_multi_atlas_support,
        tooltip_enable_multi_atlas_support,

        label_images_format,
        tooltip_images_format,

        label_images_scale,
        tooltip_images_scale,

        label_preserve_ratio_mode,
        tooltip_preserve_ratio_mode,

        label_sprites_path,
        tooltip_sprites_path,

        label_shapes2d_settings,
        tooltip_shapes2d_settings,

        label_google_fonts_settings,
        tooltip_google_fonts_settings,

        label_google_fonts_api_key,
        tooltip_google_fonts_api_key,

        label_get_google_api_key,
        tooltip_get_google_api_key,

        label_remove_from_scene,
        tooltip_remove_from_scene,

        label_token,
        tooltip_token,

        tooltip_recent_tokens,
        tooltip_auth,

        label_tmp_path,
        tooltip_tmp_path,

        label_no_recent_sessions,
        tooltip_no_recent_sessions,

        label_different_component_data,
        tooltip_different_component_data,

        label_has_differences,
        tooltip_has_differences,

        label_new_components,
        tooltip_new_components,

        label_import_frames,
        tooltip_import_frames,

        label_open_settings_window,
        tooltip_open_settings_window,

        label_select_fonts_folder,
        tooltip_select_fonts_folder,

        label_select_prefabs_folder,
        tooltip_select_prefabs_folder,

        label_max_cord_deviation,
        tooltip_max_cord_deviation,

        label_max_tangent_angle,
        tooltip_max_tangent_angle,

        label_loc_folder_path,
        tooltip_loc_folder_path,

        label_use_image_linear_material,
        tooltip_use_image_linear_material,

        label_loc_key_max_lenght,
        tooltip_loc_key_max_lenght,

        label_supported_from_unity_version,

        label_field_name_max_length,
        tooltip_field_name_max_length,

        label_method_name_max_length,
        tooltip_method_name_max_length,

        label_class_name_max_length,
        tooltip_class_name_max_length,

        label_base_class,
        tooltip_base_class,

        label_serialization_mode,
        tooltip_serialization_mode,

        label_max_sprite_size,
        tooltip_max_sprite_size,
        label_transition_type,
        tooltip_transition_type
    }
}