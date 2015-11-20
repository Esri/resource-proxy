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
                <?php $widgetSettings->getgetServiceError(); ?>
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
                <option value="<?php echo $value; ?>">
                    <?php echo $content; ?>
                </option>
            <?php } ?>
        </select>
    <?php } ?>
</span>

<span>
    <label for="<?php echo $widgetSettings->htmlId(WidgetSettings::BASE_SERVICE_URL); ?>">
        <h4><?php _ex('The base Url of the RateCalculator service (i.e. protocol, hostname, port and api path but not resource path)', 'Widget Settings', 'FpOnlineRateTable'); ?></h4>
    </label>
    <input class="widefat" type="url"
           id="<?php echo $widgetSettings->htmlId(WidgetSettings::BASE_SERVICE_URL); ?>"
           name="<?php echo $widgetSettings->htmlName(WidgetSettings::BASE_SERVICE_URL); ?>"
           value="<?php echo $widgetSettings->get(WidgetSettings::BASE_SERVICE_URL); ?>" />
</span>

<span>
    <label for="<?php echo $widgetSettings->htmlId(WidgetSettings::GET_ACTIVE_RATETABLES_PATH); ?>">
        <h4><?php _ex('The path of GetActiveTables resource', 'Widget Settings', 'FpOnlineRateTable'); ?></h4>
    </label>
    <input class="widefat" type="url"
           id="<?php echo $widgetSettings->htmlId(WidgetSettings::GET_ACTIVE_RATETABLES_PATH); ?>"
           name="<?php echo $widgetSettings->htmlName(WidgetSettings::GET_ACTIVE_RATETABLES_PATH); ?>"
           value="<?php echo $widgetSettings->get(WidgetSettings::GET_ACTIVE_RATETABLES_PATH); ?>" />
</span>

<span>
    <label for="<?php echo $widgetSettings->htmlId(WidgetSettings::RATE_CALCULATION_START_PATH); ?>">
        <h4><?php _ex('The path of RateCalculationStart resource', 'Widget Settings', 'FpOnlineRateTable'); ?></h4>
    </label>
    <input class="widefat" type="url"
           id="<?php echo $widgetSettings->htmlId(WidgetSettings::RATE_CALCULATION_START_PATH); ?>"
           name="<?php echo $widgetSettings->htmlName(WidgetSettings::RATE_CALCULATION_START_PATH); ?>"
           value="<?php echo $widgetSettings->get(WidgetSettings::RATE_CALCULATION_START_PATH); ?>" />
</span>

<span>
    <label for="<?php echo $widgetSettings->htmlId(WidgetSettings::RATE_CALCULATION_CALCULATE_PATH); ?>">
        <h4><?php _ex('The path of RateCalculationCalculate resource', 'Widget Settings', 'FpOnlineRateTable'); ?></h4>
    </label>
    <input class="widefat" type="url"
           id="<?php echo $widgetSettings->htmlId(WidgetSettings::RATE_CALCULATION_CALCULATE_PATH); ?>"
           name="<?php echo $widgetSettings->htmlName(WidgetSettings::RATE_CALCULATION_CALCULATE_PATH); ?>"
           value="<?php echo $widgetSettings->get(WidgetSettings::RATE_CALCULATION_CALCULATE_PATH); ?>" />
</span>

<span>
    <label for="<?php echo $widgetSettings->htmlId(WidgetSettings::RATE_CALCULATION_BACK_PATH); ?>">
        <h4><?php _ex('The path of RateCalculationBack resource', 'Widget Settings', 'FpOnlineRateTable'); ?></h4>
    </label>
    <input class="widefat" type="url"
           id="<?php echo $widgetSettings->htmlId(WidgetSettings::RATE_CALCULATION_BACK_PATH); ?>"
           name="<?php echo $widgetSettings->htmlName(WidgetSettings::RATE_CALCULATION_BACK_PATH); ?>"
           value="<?php echo $widgetSettings->get(WidgetSettings::RATE_CALCULATION_BACK_PATH); ?>" />
</span>

<span>
    <label for="<?php echo $widgetSettings->htmlId(WidgetSettings::RATE_CALCULATION_UPDATE_WEIGHT_PATH); ?>">
        <h4><?php _ex('The path of RateCalculationUpdateWeight resource', 'Widget Settings', 'FpOnlineRateTable'); ?></h4>
    </label>
    <input class="widefat" type="url"
           id="<?php echo $widgetSettings->htmlId(WidgetSettings::RATE_CALCULATION_UPDATE_WEIGHT_PATH); ?>"
           name="<?php echo $widgetSettings->htmlName(WidgetSettings::RATE_CALCULATION_UPDATE_WEIGHT_PATH); ?>"
           value="<?php echo $widgetSettings->get(WidgetSettings::RATE_CALCULATION_UPDATE_WEIGHT_PATH); ?>" />
</span>