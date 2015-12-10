<?php
use FP\Web\Portal\FpOnlineRateTable\src\Plugin\Widget\WidgetSettings;
?>

<span>
    <label for="<?php echo $widgetSettings->htmlId(WidgetSettings::TITLE); ?>">
        <h4><?php _ex('Widget Title', 'Widget Settings', 'FpOnlineRateTable'); ?></h4>
    </label>
    <input class="widefat" type="url"
           id="<?php echo $widgetSettings->htmlId(WidgetSettings::TITLE); ?>"
           name="<?php echo $widgetSettings->htmlName(WidgetSettings::TITLE); ?>"
           value="<?php echo $widgetSettings->get(WidgetSettings::TITLE); ?>" />
</span>

<span>
    <?php if($widgetSettings->hasServiceError()) { ?>
        <div class='error'>
            <p>
                <?php _ex('Error during contacting RateCalculation service. Please check the settings below and reload the page.',
                        'Widget Setting Error', 'FpOnlineRateTable'); ?><br>
                <?php $widgetSettings->getServiceError(); ?>
            </p>
        </div>
    <?php } else { ?>
        <label for="<?php echo $widgetSettings->htmlId(WidgetSettings::CURRENT_RATE_TABLE_CULTURE); ?>">
            <h4><?php _ex('Choose your Rate Table', 'Widget Settings', 'FpOnlineRateTable'); ?></h4>
        </label>
        <select class="widefat"
                id="<?php echo $widgetSettings->htmlId(WidgetSettings::CURRENT_RATE_TABLE_CULTURE); ?>"
                name="<?php echo $widgetSettings->htmlName(WidgetSettings::CURRENT_RATE_TABLE_CULTURE); ?>"
                value="<?php echo $widgetSettings->get(WidgetSettings::CURRENT_RATE_TABLE_CULTURE); ?>" >
            <?php foreach($widgetSettings->getRateTableChoices() as $value => $content) { ?>
                <option value="<?php echo $value; ?>"
                        <?php selected($widgetSettings->getCurrentCulture()->get(), $value); ?>>
                    <?php echo $content; ?>
                </option>
            <?php } ?>
        </select>
    <?php } ?>
</span>

<span>
    <label for="<?php echo $widgetSettings->htmlId(WidgetSettings::RESOURCE_URL); ?>">
        <h4><?php _ex('The base Url of the RateCalculator service (i.e. protocol, hostname, port and resource path but not the method path)', 'Widget Settings', 'FpOnlineRateTable'); ?></h4>
    </label>
    <input class="widefat" type="url"
           id="<?php echo $widgetSettings->htmlId(WidgetSettings::RESOURCE_URL); ?>"
           name="<?php echo $widgetSettings->htmlName(WidgetSettings::RESOURCE_URL); ?>"
           value="<?php echo $widgetSettings->get(WidgetSettings::RESOURCE_URL); ?>" />
</span>

<span>
    <label for="<?php echo $widgetSettings->htmlId(WidgetSettings::MAX_WEIGHT); ?>">
        <h4><?php _ex('Max weight in g/oz', 'Widget Settings', 'FpOnlineRateTable'); ?></h4>
    </label>
    <input class="widefat" type="number" step="1" min="0"
           id="<?php echo $widgetSettings->htmlId(WidgetSettings::MAX_WEIGHT); ?>"
           name="<?php echo $widgetSettings->htmlName(WidgetSettings::MAX_WEIGHT); ?>"
           value="<?php echo $widgetSettings->get(WidgetSettings::MAX_WEIGHT); ?>" />
</span>
